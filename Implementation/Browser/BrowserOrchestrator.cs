using Domain.Abstractions;
using Domain.Configuration;
using Domain.Dto.Mydesk;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace Implementation.Browser;

public class BrowserOrchestrator
{
    private readonly FirefoxDriver driver;
    private readonly DeskReservationOptions deskReservationOptions;

    public BrowserOrchestrator(
        DeskReservationOptions deskReservationOptions)
    {
        this.deskReservationOptions = deskReservationOptions;

        var profile = new FirefoxProfile();
        profile.SetPreference("general.useragent.override", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36");
        profile.SetPreference("layers.acceleration.disabled", true);
        profile.SetPreference("gfx.direct2d.disabled", true);

        var options = new FirefoxOptions
        {
            Profile = profile,
        };
        options.AddArguments("--headless"); // Comment out this line to debug the browser

        // Enable logging
        options.SetLoggingPreference(LogType.Browser, LogLevel.All);
        options.SetLoggingPreference(LogType.Driver, LogLevel.All);
        options.SetLoggingPreference(LogType.Performance, LogLevel.All);

        var chromeOptionsEnvVar = Environment.GetEnvironmentVariable("CHROME_OPTIONS");
        if (chromeOptionsEnvVar is not null)
        {
            options.AddArguments(chromeOptionsEnvVar);
        }
        
        this.driver = new FirefoxDriver(options);
    }

    public async Task<Result<SessionData>> GetUserSessionData()
    {
        try
        {
            this.driver
                .Navigate()
                .GoToUrl(this.deskReservationOptions.MydeskUrl);
            return await this.Login();
        }
        catch (Exception e)
        {
            return new Error(
                "BrowserOrchestrator.GetUserSessionData",
                e.Message);
        }
        finally
        {
            this.driver.Quit();
        }
    }

    private async Task<Result<SessionData>> Login()
    {
        var wait = new WebDriverWait(this.driver, TimeSpan.FromSeconds(this.deskReservationOptions.MaxWaitTimeSeconds));

        wait.Until(driver => driver.Url.Contains("login.microsoftonline.com"));
        await Task.Delay(TimeSpan.FromSeconds(6));

        var emailLoginForm = wait.Until(driver => driver.FindElement(By.CssSelector("#lightbox")));
        if (emailLoginForm is null)
        {
            return new Error(
                "BrowserOrchestrator.Login",
                "No email login form box");
        }

        var emailField = wait.Until(driver => driver.FindElements(By.CssSelector("input#i0116"))).FirstOrDefault();
        if (emailField is null)
        {
            return new Error(
                "BrowserOrchestrator.Login",
                "No email input field");
        }

        emailField.SendKeys(this.deskReservationOptions.Email);

        var emailNext = wait.Until(driver => driver.FindElements(By.CssSelector("input#idSIButton9"))).FirstOrDefault();
        if (emailNext is null)
        {
            return new Error(
                "BrowserOrchestrator.Login",
                "No email submit button");
        }

        emailNext.Click();

        // Wait for transition
        await Task.Delay(TimeSpan.FromSeconds(6));

        var passwordField = wait.Until(driver => driver.FindElements(By.CssSelector("input#i0118"))).FirstOrDefault();
        if (passwordField is null)
        {
            return new Error(
                "BrowserOrchestrator.Login",
                "No password input field");
        }

        passwordField.SendKeys(this.deskReservationOptions.Password);

        var passwordSignIn = wait.Until(driver => driver.FindElements(By.CssSelector("input#idSIButton9"))).FirstOrDefault();
        if (passwordSignIn is null)
        {
            return new Error(
                "BrowserOrchestrator.Login",
                "No signIn button");
        }

        passwordSignIn.Click();

        // Wait for transition
        await Task.Delay(TimeSpan.FromSeconds(6));

        var dontRememberMe = wait.Until(driver => driver.FindElements(By.CssSelector("input#idBtn_Back"))).FirstOrDefault();
        if (dontRememberMe is not null)
        {
            dontRememberMe.Click();
        }

        // Wait for transition.
        wait.Until(driver => driver.Url.Contains(this.deskReservationOptions.MydeskUrl));
        
        await Task.Delay(TimeSpan.FromSeconds(9));

        var js = this.driver as IJavaScriptExecutor;
        var sessionStorage = (string)js.ExecuteScript(@"let result = {};
            for(let i=0; i<sessionStorage.length; i++) {
                const key = sessionStorage.key(i);
                const value = sessionStorage.getItem(key);
                result[key] = value;
            }
            return JSON.stringify(result);");

        var keyValuePairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(sessionStorage);

        if (keyValuePairs is null)
        {
            return new Error(
                "BrowserOrchestrator.Login",
                "No session storage values");
        }

        return this.GetSessionData(keyValuePairs);
    }

    private Result<SessionData> GetSessionData(Dictionary<string, string> sessionStorage)
    {
        foreach (var sessionStorageItem in sessionStorage.Values)
        {
            var attempt = JsonConvert.DeserializeObject<SessionData>(sessionStorageItem);
            if (this.ValidateSessionData(attempt))
            {
                return attempt!;
            }
        }

        return new Error(
            "BrowserOrchestrator.GetSessionData",
            "Failed to find sessionData");
    }

    private bool ValidateSessionData(SessionData? sessionData)
    {
        if (sessionData is null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(sessionData.Secret))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(sessionData.CredentialType))
        {
            return false;
        }

        if (sessionData.CredentialType != "AccessToken")
        {
            return false;
        }

        return true;
    }
}
