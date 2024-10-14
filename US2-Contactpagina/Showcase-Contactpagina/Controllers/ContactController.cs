using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Showcase_Contactpagina.Models;
using System.Text;
using System.Text.Json.Nodes;

namespace Showcase_Contactpagina.Controllers
{
    public class ContactController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _recaptchaSecretKey;

        public ContactController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("API_HOST"));
            _recaptchaSecretKey = Environment.GetEnvironmentVariable("RecaptchaSecretKey");
        }

        // GET: ContactController
        public ActionResult Index()
        {
            return View();
        }

        // POST: ContactController
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(Contactform form)
        {
            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Er zijn fouten opgetreden bij het invullen van het formulier. Controleer of alle velden correct zijn ingevuld en probeer het opnieuw.";
                TempData["MessageType"] = "error";
                return View();
            }

            // Verify reCAPTCHA
            var recaptchaToken = Request.Form["g-recaptcha-response"];
            var verificationUrl = $"https://www.google.com/recaptcha/api/siteverify?secret={_recaptchaSecretKey}&response={recaptchaToken}";
            var verificationResponse = await _httpClient.GetStringAsync(verificationUrl);
            var verificationJson = JsonNode.Parse(verificationResponse);
            var success = verificationJson?["success"]?.GetValue<bool>() ?? false;

            if (!success)
            {
                TempData["Message"] = "Validatie van reCAPTCHA mislukt. Probeer opnieuw of neem contact op met de beheerder.";
                TempData["MessageType"] = "error";
                return View();
            }

            // Send form data
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var json = JsonConvert.SerializeObject(form, settings);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync("https://localhost:7278/api/mail", content);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Er is iets misgegaan bij het versturen van het formulier. Probeer het later opnieuw.";
                TempData["MessageType"] = "error";
                return View();
            }

            TempData["Message"] = "Bedankt! Uw bericht is succesvol verstuurd. Wij nemen spoedig contact met u op.";
            TempData["MessageType"] = "success";
            ModelState.Clear();

            return View();
        }
    }
}
