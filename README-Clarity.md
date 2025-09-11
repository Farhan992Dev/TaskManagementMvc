# راهنمای پیاده‌سازی Microsoft Clarity

Microsoft Clarity ابزار رایگان آنالیتیکس وب مایکروسافت است که امکان مشاهده رفتار کاربران از طریق heatmap، session recording و dashboard analytics را فراهم می‌کند.

## ویژگی‌های پیاده‌سازی شده

### 1. پیکربندی خودکار
- تنظیمات در `appsettings.json`
- امکان فعال/غیرفعال کردن از طریق پنل ادمین
- استفاده از Tag Helper برای بهینه‌سازی

### 2. مدیریت از پنل ادمین
- دسترسی از مسیر: `/Admin/Analytics`
- امکان مشاهده وضعیت فعلی
- راهنمای پیکربندی

### 3. اجزای پیاده‌سازی شده

#### فایل‌های اضافه شده:
- `Models/ClarityOptions.cs` - مدل تنظیمات
- `Models/ViewModels/AnalyticsSettingsViewModel.cs` - ViewModel پنل ادمین
- `TagHelpers/ClarityAnalyticsTagHelper.cs` - Tag Helper
- `Views/Admin/Analytics.cshtml` - صفحه مدیریت
- `README-Clarity.md` - این راهنما

#### فایل‌های تغییر یافته:
- `appsettings.json` - اضافه شدن تنظیمات Analytics
- `Program.cs` - ثبت Configuration
- `Controllers/AdminController.cs` - اضافه شدن متدهای مدیریت
- `Views/Shared/_Layout.cshtml` - اضافه شدن کد ردیابی
- `Views/Admin/Settings.cshtml` - اضافه شدن لینک Analytics

## راهنمای پیکربندی

### مرحله 1: دریافت Project ID از Microsoft Clarity

1. به [clarity.microsoft.com](https://clarity.microsoft.com) مراجعه کنید
2. با حساب Microsoft خود وارد شوید
3. یک پروژه جدید ایجاد کنید یا پروژه موجود را انتخاب کنید
4. وارد قسمت **Setup** شوید
5. کد ردیابی را کپی کنید. Project ID بخشی از این کد است:

```javascript
// در کد ردیابی، Project ID بخش abc123def456 است:
(function(c,l,a,r,i,t,y){
    // ...
})(window, document, "clarity", "script", "abc123def456");
```

### مرحله 2: پیکربندی در پروژه

در فایل `appsettings.json` تنظیمات زیر را اضافه/ویرایش کنید:

```json
{
  "Analytics": {
    "MicrosoftClarity": {
      "ProjectId": "YOUR_PROJECT_ID_HERE",
      "Enabled": true
    }
  }
}
```

**مثال:**
```json
{
  "Analytics": {
    "MicrosoftClarity": {
      "ProjectId": "abc123def456",
      "Enabled": true
    }
  }
}
```

### مرحله 3: راه‌اندازی مجدد

پس از تغییر `appsettings.json`، برنامه را مجدداً راه‌اندازی کنید.

### مرحله 4: تأیید عملکرد

1. به مسیر `/Admin/Analytics` مراجعه کنید
2. وضعیت پیکربندی را بررسی کنید
3. در داشبورد Clarity، ورود کاربران را مشاهده کنید

## استفاده از پنل ادمین

### دسترسی به پنل
- مسیر: `/Admin/Analytics`
- نیاز به مجوز `ManageSystem`

### ویژگی‌های پنل:
- مشاهده وضعیت فعلی تنظیمات
- ویرایش Project ID
- فعال/غیرفعال کردن ردیابی
- راهنمای پیکربندی
- لینک مستقیم به داشبورد Clarity

## ویژگی‌های فنی

### Tag Helper
از Tag Helper سفارشی برای بهینه‌سازی استفاده شده:

```html
<!-- در Layout -->
<clarity-analytics />

<!-- یا با تنظیمات سفارشی -->
<clarity-analytics project-id="custom-id" enabled="true" />
```

### Configuration Binding
تنظیمات به صورت خودکار از `appsettings.json` خوانده می‌شوند:

```csharp
// در Program.cs
builder.Services.Configure<AnalyticsOptions>(
    builder.Configuration.GetSection("Analytics"));
```

### Conditional Loading
کد ردیابی فقط در صورت فعال بودن و وجود Project ID لود می‌شود.

## حریم خصوصی و امنیت

### ویژگی‌های امنیتی Clarity:
- خودکار پنهان کردن اطلاعات حساس (رمز عبور، شماره کارت، etc.)
- رعایت قوانین GDPR
- عدم ذخیره IP دقیق کاربران
- رمزگذاری داده‌ها

### تنظیمات پیشنهادی:
در Clarity Dashboard تنظیمات زیر را بررسی کنید:
- Mask sensitive content: فعال
- Cookie consent compliance: مطابق قوانین محلی
- Data retention: مطابق نیاز

## مشکلات رایج

### 1. کد ردیابی لود نمی‌شود
- بررسی کنید Project ID صحیح باشد
- `Enabled` در تنظیمات `true` باشد
- Console browser را برای خطاها بررسی کنید

### 2. داده‌ها در Dashboard نمایش داده نمی‌شوند
- حداقل 30 دقیقه صبر کنید (تأخیر معمول Clarity)
- URL سایت در Clarity صحیح تنظیم شده باشد
- فیلترهای Ad Blocker را بررسی کنید

### 3. خطای Configuration
- فرمت JSON در `appsettings.json` صحیح باشد
- پس از تغییر، برنامه راه‌اندازی مجدد شده باشد
- سطح دسترسی کاربر ادمین صحیح باشد

## منابع مفید

- [Microsoft Clarity Documentation](https://docs.microsoft.com/en-us/clarity/)
- [Clarity Dashboard](https://clarity.microsoft.com)
- [Privacy and GDPR Compliance](https://docs.microsoft.com/en-us/clarity/privacy)

## ساپورت

برای مشکلات فنی:
1. ابتدا این راهنما را مطالعه کنید
2. Console browser را برای خطاها بررسی کنید  
3. تنظیمات داشبورد Clarity را بررسی کنید
4. در صورت نیاز با تیم توسعه تماس بگیرید
