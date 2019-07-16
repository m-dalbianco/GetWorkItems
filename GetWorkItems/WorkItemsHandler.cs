using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http.Headers;
using System.Net.Http;
using GetWorkItems.Model.Dtos;
using Newtonsoft.Json.Linq;

namespace GetWorkItems {
    public class WorkItemsHandler {
        private ConfigurationDto _configuration;

        public WorkItemsHandler() {
            FillConfiguration();
        }

        private void FillConfiguration() {
            Console.WriteLine("Configurando a requisição");

            using (var db = new GetWorkItemContext()) {
                var configurationDto = db.Configurations
                     .Select(c => new ConfigurationDto {
                         Url = c.Url,
                         ProjectName = c.ProjectName,
                         Token = c.Token
                     })
                     .FirstOrDefault();

                ValidateConfiguration(configurationDto);

                _configuration = new ConfigurationDto {
                    ProjectName = configurationDto.ProjectName,
                    Token = configurationDto.Token,
                    Url = configurationDto.Url
                };
            }
        }

        private void ValidateConfiguration(ConfigurationDto configuration) {
            if (configuration == null) {
                throw new Exception("É necessário realizar a configuração no web site para realizar a operação.");
            }
        }

        private HttpClient ConfiguredHttpClient() {
            var client = new HttpClient {
                BaseAddress = new Uri(_configuration.Url)
            };

            string token = $"{ string.Empty }:{ _configuration.Token }";
            string encodedToken = Convert.ToBase64String(Encoding.ASCII.GetBytes(token));

            client.DefaultRequestHeaders
                  .Authorization = new AuthenticationHeaderValue("Basic", encodedToken);

            return client;
        }

        public WorkItemDto[] GetWorkItems() {
            var workItemsDto = new HashSet<WorkItemDto>();
            var ids = GetWorkItemsIds();
            var apiResponse = new JObject();

            Console.WriteLine("Obtendo as propriedados dos work items");

            using (var client = ConfiguredHttpClient()) {
                string[] fields = new string[] {
                    "System.Id",
                    "System.Title",
                    "System.WorkItemType",
                    "System.CreatedDate"
                };

                string requestURI = "_apis/wit/workitems?ids=" + string.Join(",", ids) +
                                    "&fields=" + string.Join(",", fields) +
                                    "&api-version=4.1";
                try {
                    using (HttpResponseMessage response = client.GetAsync(requestURI).Result) {
                        response.EnsureSuccessStatusCode();
                        string responseBody = response.Content.ReadAsStringAsync().Result;
                        apiResponse = JObject.Parse(responseBody);
                    }
                } catch (Exception ex) {
                    Console.WriteLine(ex.ToString());
                }
            }

            foreach (dynamic workItem in (JArray)apiResponse["value"]) {
                var field = (JObject)workItem.fields ;
                fillWorkItem(field);
            }

            return workItemsDto.ToArray();      
            
            void fillWorkItem(JObject field) {
                workItemsDto.Add(new WorkItemDto {
                    IdWorkItem = (int)field.GetValue("System.Id"),
                    CreatedDate = (DateTime)field.GetValue("System.CreatedDate"),
                    Title = (string)field.GetValue("System.Title"),
                    WorkItemType = (string)field.GetValue("System.WorkItemType"),
                });
            }
        }

        private int[] GetWorkItemsIds() {
            Console.WriteLine("Obtendo os ids dos work items");

            var ids = new HashSet<int>();
            var apiResponse = new JObject();

            using (var client = ConfiguredHttpClient()) {
                var query = "Select [System.Id] From WorkItems";
                var requestBody = new StringContent("{ \"query\": \"" + query + "\" }",
                    Encoding.UTF8, "application/json");

                try {
                     using (HttpResponseMessage response = client.PostAsync("_apis/wit/wiql?api-version=5.0", requestBody).Result) {
                        response.EnsureSuccessStatusCode();
                        string responseBody = response.Content.ReadAsStringAsync().Result;
                        apiResponse = JObject.Parse(responseBody);
                    }
                } catch (Exception ex) {
                    Console.WriteLine(ex.ToString());
                }

                JArray workItems = (JArray)apiResponse["workItems"];

                foreach (dynamic workItem in workItems) {
                    ids.Add((int)workItem.id);
                }

                return ids.ToArray();
            }
        }
    }
}
