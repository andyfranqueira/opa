// <copyright file="Utilities.cs" company="Anargyroi Development">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of Online Parish Administration.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OPA.DataAccess;
using OPA.Entities;

namespace OPA.BusinessLogic
{
    public static class Utilities
    {
        public static readonly string AdminEmail = ConfigurationManager.AppSettings["admin:Email"];
        private static readonly string AdminName = ConfigurationManager.AppSettings["admin:Name"];
        private static readonly MailAddress MailFrom = new MailAddress(AdminEmail, AdminName);
        private static readonly string SmtpHost = ConfigurationManager.AppSettings["smtp:Host"];
        private static readonly string SmtpPort = ConfigurationManager.AppSettings["smtp:Port"];
        private static readonly string SmtpAccount = ConfigurationManager.AppSettings["smtp:Account"];
        private static readonly string SmtpPassword = ConfigurationManager.AppSettings["smtp:Password"];
        private static readonly string[] ImageFileExtensions = { ".jpg", ".png", ".gif", ".jpeg" };

        public static void InitializeDatabase()
        {
            var database = new OpaContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(database));
            var adminUser = userManager.FindByName(AdminEmail);

            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = AdminEmail,
                    Email = AdminEmail,
                    EmailConfirmed = true
                };

                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(database));
                if (!roleManager.Roles.Any())
                {
                    roleManager.Create(new IdentityRole { Name = "Admin" });
                    roleManager.Create(new IdentityRole { Name = "User" });
                }

                userManager.Create(user, DateTime.Now.ToString(CultureInfo.InvariantCulture));
                userManager.AddToRole(user.Id, "Admin");
            }
        }

        public static void EncryptConfigSection(string sectionKey)
        {
            var config = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
            var section = config.GetSection(sectionKey);
            if (section != null)
            {
                if (!section.SectionInformation.IsProtected)
                {
                    if (!section.ElementInformation.IsLocked)
                    {
                        section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
                        section.SectionInformation.ForceSave = true;
                        config.Save(ConfigurationSaveMode.Full);
                    }
                }
            }
        }

        public static Task AsyncSendEmail(string[] to, string subject, string body)
        {
            var mailMessage = new MailMessage
            {
                From = MailFrom,
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            foreach (var address in to)
            {
                mailMessage.To.Add(address);
            }

            return EmailClient().SendMailAsync(mailMessage);
        }

        public static string FormatName(Person person)
        {
            if (person == null)
            {
                return string.Empty;
            }

            return person.FirstName + " " + person.MiddleName + " " + person.LastName;
        }

        public static string FormatAddress(Address address)
        {
            return address.AddressLine +
                   ", " + address.City +
                   ", " + address.State +
                   ", " + address.PostalCode +
                   (address.Country != null ? ", " + address.Country : string.Empty);
        }

        public static bool IsImageFile(HttpPostedFileBase file)
        {
            return file != null 
                && (file.ContentType.Contains("image") 
                || ImageFileExtensions.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase)));
        }

        public static Image CropImage(Image image, int x, int y, int width, int height)
        {
            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
            bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            var graphic = Graphics.FromImage(bitmap);
            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.DrawImage(
                image,
                new Rectangle(0, 0, width, height),
                new Rectangle(x, y, width, height),
                GraphicsUnit.Pixel);

            graphic.Dispose();
            return bitmap;
        }

        public static Image ScaleImage(Image image, int width, int height)
        {
            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
            bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            var graphic = Graphics.FromImage(bitmap);
            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.DrawImage(
                image,
                new Rectangle(0, 0, width, height),
                new Rectangle(0, 0, image.Width, image.Height),
                GraphicsUnit.Pixel);

            graphic.Dispose();
            return bitmap;
        }

        public static Image ScaleImage(Image image, int size)
        {
            var width = size;
            var height = size;

            if (image.Height > image.Width)
            {
                width = size * image.Width / image.Height;
            }
            else
            {
                height = image.Height * size / image.Width;
            }

            return ScaleImage(image, width, height);
        }

        public static string ConvertImageToBase64(Image image, ImageFormat format)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, format);
                return "data:image/jpg;base64," + Convert.ToBase64String(ms.ToArray());
            }
        }

        public static Image ConvertBase64ToImage(string image64Bit)
        {
            var imageBytes = Convert.FromBase64String(image64Bit.Replace("data:image/jpg;base64,", string.Empty));
            return new Bitmap(new MemoryStream(imageBytes));
        }

        public static byte[] GetFile(string file)
        {
            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Delete | FileShare.ReadWrite))
            {
                var length = System.Convert.ToInt32(stream.Length);
                var data = new byte[length];
                stream.Read(data, 0, length);
                return data;
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static byte[] GenerateWordDocument(string template, Dictionary<string, string> fieldValues)
        {
            using (var memoryStream = new MemoryStream())
            {
                var byteArray = File.ReadAllBytes(template);
                memoryStream.Write(byteArray, 0, byteArray.Length);

                using (var document = WordprocessingDocument.Open(memoryStream, true))
                {
                    string text;
                    using (var streamReader = new StreamReader(document.MainDocumentPart.GetStream()))
                    {
                        text = streamReader.ReadToEnd();
                    }

                    foreach (var fieldValue in fieldValues)
                    {
                        var regexText = new Regex(fieldValue.Key);
                        text = regexText.Replace(text, fieldValue.Value);
                    }

                    using (var streamWriter = new StreamWriter(document.MainDocumentPart.GetStream(FileMode.Create)))
                    {
                        streamWriter.Write(text);
                    }

                    document.Save();
                }

                return memoryStream.ToArray();
            }
        }

        private static SmtpClient EmailClient()
        {
            return new SmtpClient
            {
                Host = SmtpHost,
                Port = Convert.ToInt32(SmtpPort),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(SmtpAccount, SmtpPassword)
            };
        }
    }
}
