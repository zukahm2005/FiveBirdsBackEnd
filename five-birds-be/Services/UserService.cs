using System.Web;
using DeviceDetectorNET;
using five_birds_be.Data;
using five_birds_be.Dto;
using five_birds_be.DTO.Request;
using five_birds_be.DTO.Response;
using five_birds_be.Jwt;
using five_birds_be.Models;
using five_birds_be.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace five_birds_be.Services
{
    public class UserService
    {
        private DataContext _dataContext;
        private JwtService _jservice;
        private IMemoryCache _cache;
        private IHttpContextAccessor _httpContextAccessor;
        private EmailService _emailService;
        private IConfiguration _iconfiguration;
        public UserService(DataContext dataContext, JwtService jservice, IHttpContextAccessor httpContextAccessor, EmailService emailService, IMemoryCache cache, IConfiguration configuration)
        {
            _dataContext = dataContext;
            _jservice = jservice;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
            _cache = cache;
            _iconfiguration = configuration;

        }

        public async Task<List<User>> GetUsersPaged(int pageNumber)
        {
            if (pageNumber < 1) pageNumber = 1;

            return await _dataContext.Users
                .Skip((pageNumber - 1) * 10)
                .Take(10)
                .ToListAsync();
        }

        public async Task<ApiResponse<string>> Register(UserDTO userDTO)
        {
            var existingUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == userDTO.Email);
            if (existingUser != null)
            {
                return ApiResponse<string>.Failure(400, "Email already in use.");
            }
            var temporaryUser = new User
            {
                UserName = userDTO.UserName,
                Email = userDTO.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(userDTO.Password),
                Create_at = DateTime.UtcNow
            };

            _cache.Set(userDTO.Email, temporaryUser, TimeSpan.FromMinutes(5));

            var otp = new Random().Next(100000, 999999).ToString();
            var hashedOtp = BCrypt.Net.BCrypt.HashPassword(otp);

            _cache.Set($"otp:{userDTO.Email}", hashedOtp, TimeSpan.FromMinutes(5));

            var subject = "Account registration successful";
            var body = $@"
                    <html>
                        <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0;'>
                            <div style='max-width: 600px; margin: 30px auto; background-color: #ffffff; padding: 20px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);'>
                                <h2 style='color: #333;'>Hello,</h2>
                                <p style='font-size: 16px; color: #555;'>Your email verification code is:</p>
                                <div style='width: 100%; height: 50px; display: flex; justify-content: center; align-items: center;'>
                                    <span style='font-size: 24px; padding: 10px 20px; font-weight: bold; color: #fff; border-radius: 5px; text-align: center; background-color: #4CAF50;'>
                                        {otp}
                                    </span>
                                </div>
                                <p style='font-size: 16px; color: #555;'>This code will expire in 5 minutes.</p>
                                <p style='font-size: 16px; color: #555;'>Thank you for using our service!</p>
                            </div>
                        </body>
                    </html>";
            await _emailService.SendEmailAsync(userDTO.Email, subject, body);

            return ApiResponse<string>.Success(200, null, "Account created successfully and confirmation email sent.");
        }


        public async Task<ApiResponse<string>> VerifyEmail(VerifyEmail email)
        {
            if (!_cache.TryGetValue($"otp:{email.Email}", out string cachedHashedOtp))
            {
                return ApiResponse<string>.Failure(400, "OTP has expired.");
            }
            if (!BCrypt.Net.BCrypt.Verify(email.Otp, cachedHashedOtp))
            {
                return ApiResponse<string>.Failure(400, "Invalid OTP.");
            }
            if (!_cache.TryGetValue(email.Email, out User temporaryUser))
            {
                return ApiResponse<string>.Failure(400, "Temporary user data not found or expired.");
            }

            _cache.Remove($"otp:{email.Email}");
            _cache.Remove(email.Email);

            await _dataContext.Users.AddAsync(temporaryUser);
            await _dataContext.SaveChangesAsync();

            return ApiResponse<string>.Success(200, null, "Email verified and account created successfully.");
        }



        public async Task<ApiResponse<UserResponseDTO>> UpdateUser(UserDTO userDTO)
        {
            var userId = _jservice.GetUserIdFromHttpContext();
            var user = await _dataContext.Users.FindAsync(userId);

            if (user == null) return ApiResponse<UserResponseDTO>.Failure(404, " UserId NotFound");

            if (!BCrypt.Net.BCrypt.Verify(userDTO.Password, user.Password))
                return ApiResponse<UserResponseDTO>.Failure(400, "Incorrect password");

            if (!string.IsNullOrEmpty(userDTO.NewPassword))
            {
                if (userDTO.NewPassword.Length < 6)
                    return ApiResponse<UserResponseDTO>.Failure(400, "Password must be at least 6 characters long");

                user.Password = BCrypt.Net.BCrypt.HashPassword(userDTO.NewPassword);
            }

            user.UserName = userDTO.UserName;
            user.Email = userDTO.Email;
            user.Create_at = user.Create_at;
            user.Update_at = DateTime.Now;

            var userResponseDTO = new UserResponseDTO
            {
                UserName = user.UserName,
                Email = user.Email,
                Create_at = user.Create_at,
                Update_at = user.Update_at,
            };

            await _dataContext.SaveChangesAsync();
            return ApiResponse<UserResponseDTO>.Success(200, userResponseDTO, "Updata user success");
        }

        public async Task<ApiResponse<string>> Login(UserLoginDTO userDTO)
        {
            var user = await _dataContext.Users
                .FirstOrDefaultAsync(u => u.UserName == userDTO.UserName);

            if (user == null)
                return ApiResponse<string>.Failure(404, "Username not found");

            if (!BCrypt.Net.BCrypt.Verify(userDTO.Password, user.Password))
                return ApiResponse<string>.Failure(400, "Incorrect password");

            var userAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString();
            var deviceDetector = new DeviceDetectorNET.DeviceDetector(userAgent);
            deviceDetector.Parse();

            var deviceInfo = userAgent;

            var trustedDevice = await _dataContext.TrustedDevices
                .FirstOrDefaultAsync(d => d.DeviceIdentifier == deviceInfo && d.UserId == user.UserId);

            if (trustedDevice != null)
            {
                if (!trustedDevice.IsTrusted)
                {
                    if (trustedDevice.TrustedUntil > DateTime.UtcNow)
                    {
                        if (trustedDevice.LastEmailSent == null || (DateTime.UtcNow - trustedDevice.LastEmailSent.Value).TotalSeconds > 60)
                        {
                            var changeDeviceTrustLink = GenerateDeviceTrustLink(user.UserId, deviceInfo);
                            await SendDeviceVerificationEmail(user, deviceInfo, changeDeviceTrustLink);

                            trustedDevice.LastEmailSent = DateTime.UtcNow;
                            _dataContext.TrustedDevices.Update(trustedDevice);
                            await _dataContext.SaveChangesAsync();

                            return ApiResponse<string>.Failure(400, "Device not approved.");
                        }
                        return ApiResponse<string>.Failure(400, "You can only request a new verification email every 60 seconds.");
                    }

                    return ApiResponse<string>.Failure(400, "Device trust expired.");

                }
            }
            else
            {
                await AddNewUntrustedDevice(user.UserId, deviceInfo);
                var changeDeviceTrustLink = GenerateDeviceTrustLink(user.UserId, deviceInfo);
                await SendDeviceVerificationEmail(user, deviceInfo, changeDeviceTrustLink);
                return ApiResponse<string>.Failure(403, "Login from untrusted device is not allowed.");
            }

            var token = _jservice.GenerateJwtToken(user);
            SetAuthCookie(token);

            return ApiResponse<string>.Success(200, token, "User logged in successfully.");
        }

        private async Task AddNewUntrustedDevice(int userId, string deviceInfo)
        {
            var newDevice = new TrustedDevice
            {
                UserId = userId,
                DeviceIdentifier = deviceInfo,
                IsTrusted = false,
                TrustedUntil = DateTime.UtcNow.AddMonths(1)
            };

            _dataContext.TrustedDevices.Add(newDevice);
            await _dataContext.SaveChangesAsync();
        }

        private string GenerateDeviceTrustLink(int userId, string deviceInfo)
        {
            var encodedDeviceInfo = Uri.EscapeDataString(deviceInfo);
            var redirectUrl = Uri.EscapeDataString("http://localhost:5173/login");
            return $"{_iconfiguration["AppSettings:BaseUrl"]}/api/v1/users/change-device-trust?userId={userId}&deviceInfo={encodedDeviceInfo}&trust=true&redirectUrl={redirectUrl}";
        }


        private async Task SendDeviceVerificationEmail(User user, string deviceInfo, string changeDeviceTrustLink)
        {

            var emailBody = $@"
                    <html>
                        <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0;'>
                            <div style='max-width: 600px; margin: 30px auto; background-color: #ffffff; padding: 20px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);'>
                                <h2 style='color: #333;'>Hello {user.UserName},</h2>
                                <p style='color: #555; font-size: 16px;'>We detected a login attempt from a new device:</p>
                                <p style='color: #555; font-size: 16px;'><b>Device:</b> {deviceInfo}</p>
                                <p style='color: #555; font-size: 16px;'><b>Time:</b> {DateTime.UtcNow}</p>
                                <p>
                                    <a href='{changeDeviceTrustLink}' style='padding: 10px 20px; background-color: #4CAF50; color: white; text-decoration: none; border-radius: 5px;'>Trust This Device</a>
                                </p>
                            </div>
                        </body>
                    </html>";


            if (!string.IsNullOrEmpty(user.Email))
                await _emailService.SendEmailAsync(user.Email, "New Device Login Attempt", emailBody);
        }

        private void SetAuthCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            _httpContextAccessor.HttpContext.Response.Cookies.Append("token", token, cookieOptions);
        }

        public async Task<ApiResponse<UserResponseDTO>> GetUserById()
        {
            var userId = _jservice.GetUserIdFromHttpContext();
            if (userId == null) return ApiResponse<UserResponseDTO>.Failure(404, "userid not found");
            Console.WriteLine($"userId: {userId}");

            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return ApiResponse<UserResponseDTO>.Failure(404, "user null");

            var userResponseDTO = new UserResponseDTO
            {
                UserName = user.UserName,
                Email = user.Email,
                Create_at = user.Create_at,
                Update_at = user.Update_at,
            };

            return ApiResponse<UserResponseDTO>.Success(200, userResponseDTO, "get User id success");
        }

        public async Task<ApiResponse<string>> ForgotPassword(ForgotPasswordRequest request)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null) return ApiResponse<string>.Failure(404, "Email does not exist.");

            var otp = new Random().Next(100000, 999999).ToString();

            var hashedOtp = BCrypt.Net.BCrypt.HashPassword(otp);
            _cache.Set(user.Email, hashedOtp, TimeSpan.FromMinutes(5));

            var subject = "Reset Password OTP";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0;'>
                    <div style='max-width: 600px; margin: 30px auto; background-color: #ffffff; padding: 20px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);'>
                        <h2 style='color: #333;'>Hello,</h2>
                        <p style='font-size: 16px; color: #555;'>Your OTP code to reset your password is:</p>
                        <div style='width: 100%; height: 50px; display: flex; justify-content: center; align-items: center;'>
                            <span style='font-size: 24px; padding: 10px 20px; font-weight: bold; color: #fff; border-radius: 5px; text-align: center; background-color: #4CAF50;'>
                                {otp}
                            </span>
                        </div>
                        <p style='font-size: 16px; color: #555;'>This code will expire in 5 minutes.</p>
                        <p style='font-size: 16px; color: #555;'>Thank you for using our service!</p>
                    </div>
                </body>
                </html>";


            await _emailService.SendEmailAsync(user.Email, subject, body);

            return ApiResponse<string>.Success(200, null, "An OTP has been sent to your email.");
        }

        public async Task<ApiResponse<string>> VerifyOtpAndResetPassword(VerifyOtpRequest request)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null) return ApiResponse<string>.Failure(404, "Email does not exist.");

            if (!_cache.TryGetValue(request.Email, out string cachedHashedOtp))
            {
                return ApiResponse<string>.Failure(400, "OTP has expired.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Otp, cachedHashedOtp))
            {
                return ApiResponse<string>.Failure(400, "Invalid OTP.");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.Password = hashedPassword;

            _dataContext.Users.Update(user);
            await _dataContext.SaveChangesAsync();

            _cache.Remove(request.Email);

            return ApiResponse<string>.Success(200, null, "Password has been successfully reset.");
        }
        public User ConvertToEntity(UserResponseDTO userResponseDTO)
        {
            return new User
            {
                UserName = userResponseDTO.UserName,
                Email = userResponseDTO.Email,
            };
        }

        public async Task<ApiResponse<string>> ChangeDeviceTrust(int userId, string deviceInfo, bool trust)
        {
            var device = await _dataContext.TrustedDevices
                .FirstOrDefaultAsync(d => d.UserId == userId && d.DeviceIdentifier == deviceInfo);
            var dataUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            if (device == null)
            {
                return ApiResponse<string>.Failure(404, "Device not found.");
            }
            device.IsTrusted = trust;
            if (trust)
            {
                device.TrustedUntil = DateTime.UtcNow.AddMonths(1);
            }
            else
            {
                device.TrustedUntil = DateTime.UtcNow;
            }

            await _dataContext.SaveChangesAsync();

            var confirmationEmailBody = $@"
                <p>Hello {dataUser.UserName},</p>
                <p>The device with identifier <b>{deviceInfo}</b> has been successfully trusted.</p>
                <p>You can now login from this device without additional verification.</p>";

            await _emailService.SendEmailAsync(dataUser.Email, "Device Trusted Successfully", confirmationEmailBody);

            return ApiResponse<string>.Success(200, null, $"Device trust status changed to {trust}");
        }



    }
}