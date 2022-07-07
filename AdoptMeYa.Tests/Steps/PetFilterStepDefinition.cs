using TechTalk.SpecFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using GrupoWebBackend;
using GrupoWebBackend.DomainPets.Resources;
using GrupoWebBackend.Security.Resources;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using NUnit.Framework;
using SpecFlow.Internal.Json;
using TechTalk.SpecFlow.Assist;

namespace AdoptMeYa.Tests.Steps
{
    [Binding]
    public class PetFilterStepDefinition
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private HttpClient Client { get; set; }
        private Uri _baseUri;
        private PetResource Pet { get; set; }
        private UserResource User { get; set; }
        //private ConfiguredTaskAwaitable<HttpResponseMessage> Response { get; set; }
        private Task<HttpResponseMessage> Response { get; set; }
        private string _type { get; set; }
        private string _gender { get; set; }
        private string _attention { get; set; }
        private bool isGenderAvailable { get; set; }
        private bool isAttentionAvailable { get; set; }
        
        public PetFilterStepDefinition(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }
        
        [Given(@"The endpoint https://localhost:(.*)/api/v(.*)/Pets is available")]
        public void GivenTheEndpointHttpsLocalhostApiVPetsIsAvailable(int port, int version)
        {
            _baseUri = new Uri($"https://localhost:{port}/api/v{version}/Pets");
            Client = _factory.CreateClient(new WebApplicationFactoryClientOptions{BaseAddress = _baseUri});
        }
        
        [Given(@"A User is already Stored")]
        public async void GivenAUserIsAlreadyStored(Table saveUserResourceData)
        {
            var saveUserResource = saveUserResourceData.CreateSet<SavePetResource>().First();
            var userUri = new Uri("https://localhost:5001/api/v1/Users");
            var content = new StringContent(saveUserResource.ToJson(), Encoding.UTF8, MediaTypeNames.Application.Json);
            var userResponse = Client.PostAsync(userUri, content);
            var userResponseData = await userResponse.Result.Content.ReadAsStringAsync();
            var existingUser = JsonConvert.DeserializeObject<UserResource> (userResponseData);
            User = existingUser;
        }
        
        [Given(@"A pet is already stored")]
        public async void GivenAPetIsAlreadyStored(Table savePetResourceData)
        {
            var savePetResource = savePetResourceData.CreateSet<SavePetResource>().First();
            var petUri = new Uri("https://localhost:5001/api/v1/Pets");
            var content = new StringContent(savePetResource.ToJson(), Encoding.UTF8, MediaTypeNames.Application.Json);
            var petResponse = Client.PostAsync(petUri, content);
            var petResponseData = await petResponse.Result.Content.ReadAsStringAsync();
            var existingPet = JsonConvert.DeserializeObject<PetResource> (petResponseData);
            Pet = existingPet;
        }

        [Given(@"the Endpoint https://localhost:(.*)/api/v(.*)/Pets/type=(.*)/gender=(.*)/attention=(.*) is available")]
        public void GivenTheEndpointHttpsLocalhostApiVPetsTypeGenderAttentionIsAvailable(int port, int version, string type, string gender, string attention)
        {
            _baseUri = new Uri($"https://localhost:{port}/api/v{version}/Pets/type={type}/gender={gender}/attention={attention}");
            Client = _factory.CreateClient(new WebApplicationFactoryClientOptions{BaseAddress = _baseUri});
            _type = type;
            _gender = gender;
            _attention = attention;

            isGenderAvailable = true;
            isAttentionAvailable = true;
        }
        
        [Given(@"the endpoint https://localhost:(.*)/api/v(.*)/Pets/type=(.*) is available")]
        public void GivenTheEndpointHttpsLocalhostApiVPetsTypeIsAvailable(int port, int version, string type)
        {
            _baseUri = new Uri($"https://localhost:{port}/api/v{version}/Pets/type={type}");
            Client = _factory.CreateClient(new WebApplicationFactoryClientOptions{BaseAddress = _baseUri});
            _type = type;
            isGenderAvailable = false;
            isAttentionAvailable = false;
        }

        [When(@"a get request with filters is sent")]
        public void WhenAGetRequestWithFiltersIsSent()
        {
            Response = Client.GetAsync(_baseUri);
        }

        private void AssertExpectedTypeEqualResponseType(PetResource expectedResource, PetResource response)
        {
            Assert.AreEqual(expectedResource.Type, response.Type);
        }
        
        private void AssertExpectedAttentionEqualResponseAttention(PetResource expectedResource, PetResource response)
        {
            Assert.AreEqual(expectedResource.Attention, response.Attention);
        }
        
        private void AssertExpectedGenderEqualResponseGender(PetResource expectedResource, PetResource response)
        {
            Assert.AreEqual(expectedResource.Gender, response.Gender);
        }
        
        [Then(@"A Pet Resource is included in Response Body with one Filter")]
        public async void ThenAPetResourceIsIncludedInResponseBodyWithOneFilter(Table expectedProductResource)
        {
            var expectedResource = expectedProductResource.CreateSet<PetResource>().First();
            var responseData = await Response.Result.Content.ReadAsStringAsync();
            var resource = JsonConvert.DeserializeObject<List<PetResource>>(responseData);

            AssertExpectedTypeEqualResponseType(expectedResource, resource.First());
        }
        
        [Then(@"A Pet Resource is included in Response Body with many Filters")]
        public async void ThenAPetResourceIsIncludedInResponseBodyWithManyFilters(Table expectedProductResource)
        {
            var expectedResource = expectedProductResource.CreateSet<PetResource>().First();
            var responseData = await Response.Result.Content.ReadAsStringAsync();
            var resource = JsonConvert.DeserializeObject<List<PetResource>>(responseData);

            AssertExpectedTypeEqualResponseType(expectedResource, resource.First());
            AssertExpectedAttentionEqualResponseAttention(expectedResource, resource.First());
            AssertExpectedGenderEqualResponseGender(expectedResource, resource.First());
        }

        [Then(@"An empty list is included in Response Body")]
        public async void ThenAnEmptyListIsIncludedInResponseBody()
        {
            var responseData = await Response.Result.Content.ReadAsStringAsync();
            var resource = JsonConvert.DeserializeObject<List<PetResource>>(responseData);

            Assert.AreEqual(0, resource.Count);
        }
    }
}