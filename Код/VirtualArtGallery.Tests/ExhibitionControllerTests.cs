using NUnit.Framework;
using NUnit.Framework.Legacy;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.Diagnostics;

namespace ExhibitionTests
{
    [TestFixture]
    public class ExhibitionControllerTests : IDisposable
    {
        private IWebDriver driver;
        private Process serverProcess;

        private const string ProjectPath = @"D:\Коды и чертежи\ТП\5 семестр\Курсовик\Virtual-Art-Gallery\Код\Virtual-Art-Gallery рабочий";
        private const string BaseUrl = "http://localhost:5001";

        [SetUp]
        public void Setup()
        {
            StartAspNetCoreApp();
            WaitForServer();

            var options = new FirefoxOptions();
            // options.AddArgument("-headless"); // Для выполнения тестов в безголовом режиме
            driver = new FirefoxDriver(options);

            Thread.Sleep(10000); // Задержка для запуска сервера
        }

        [Test]
        public void TestLogin()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

            driver.Navigate().GoToUrl(BaseUrl + "/Identity/Account/Login");

            var usernameField = wait.Until(d => d.FindElement(By.Id("Input_Username")));
            var passwordField = wait.Until(d => d.FindElement(By.Id("Input_Password")));
            var loginButton = wait.Until(d => d.FindElement(By.Id("login-submit")));

            usernameField.SendKeys("Admin");
            passwordField.SendKeys("Admin1!");
            loginButton.Click();

            Thread.Sleep(5000);

            ClassicAssert.IsTrue(driver.PageSource.Contains("Здравствуйте, Admin!"));
        }

        [Test]
        public void TestViewAllExhibitions()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            driver.Navigate().GoToUrl(BaseUrl + "/Exhibition");

            var exhibitionsTable = wait.Until(d => d.FindElement(By.CssSelector(".table")));
            ClassicAssert.IsTrue(exhibitionsTable.Displayed, "Таблица выставок не отображается.");

            var rows = exhibitionsTable.FindElements(By.TagName("tr"));
            ClassicAssert.IsTrue(rows.Count > 1, "В таблице нет записей о выставках.");

            var headerRow = rows[0];
            var firstDataRow = rows[1];

            var headers = headerRow.FindElements(By.TagName("th"));
            ClassicAssert.AreEqual("Название", headers[0].Text, "Заголовок первой колонки не совпадает.");
            ClassicAssert.AreEqual("Описание", headers[1].Text, "Заголовок второй колонки не совпадает.");

            var cells = firstDataRow.FindElements(By.TagName("td"));
            ClassicAssert.IsTrue(!string.IsNullOrEmpty(cells[0].Text), "Название выставки не заполнено в первой строке.");
            ClassicAssert.IsTrue(!string.IsNullOrEmpty(cells[1].Text), "Описание не заполнено в первой строке.");

            Console.WriteLine("Первая строка таблицы:");
            foreach (var cell in cells)
            {
                Console.WriteLine(cell.Text);
            }
        }

        [Test]
        public void TestSpecificExhibitionDetails()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

            driver.Navigate().GoToUrl(BaseUrl + "/Identity/Account/Login");

            var usernameField = wait.Until(d => d.FindElement(By.Id("Input_Username")));
            var passwordField = wait.Until(d => d.FindElement(By.Id("Input_Password")));
            var loginButton = wait.Until(d => d.FindElement(By.Id("login-submit")));

            usernameField.SendKeys("Admin");
            passwordField.SendKeys("Admin1!");
            loginButton.Click();

            driver.Navigate().GoToUrl(BaseUrl + "/Exhibition");

            var exhibitionLink = wait.Until(d => d.FindElement(By.CssSelector("a[href='/Exhibition/Details/1']")));
            exhibitionLink.Click();

            Console.WriteLine(driver.PageSource);
        }

        [TearDown]
        public void TearDown()
        {
            Dispose();
        }

        public void Dispose()
        {
            driver?.Quit();
            driver?.Dispose();

            if (serverProcess != null && !serverProcess.HasExited)
            {
                serverProcess.Kill();
                serverProcess.Dispose();
            }
        }

        private void StartAspNetCoreApp()
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "run",
                WorkingDirectory = ProjectPath,
                UseShellExecute = true,
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                CreateNoWindow = true
            };

            serverProcess = Process.Start(startInfo);
        }
        private void WaitForServer()
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                for (int i = 0; i < 30; i++) // Попробовать подключиться 30 раз
                {
                    try
                    {
                        var response = client.GetAsync(BaseUrl).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            return; // Сервер успешно запущен
                        }
                    }
                    catch
                    {
                        Thread.Sleep(1000); // Подождать 1 секунду перед повторной попыткой
                    }
                }
                throw new Exception("Сервер не запустился за отведенное время.");
            }
        }


    }
}
