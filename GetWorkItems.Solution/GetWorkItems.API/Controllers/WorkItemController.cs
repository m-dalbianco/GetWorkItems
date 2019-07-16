using System.Linq;
using AutoMapper;
using GetWorkItems.API.Returns;
using GetWorkItems.Model.Dtos;
using GetWorkItems.Model.Services;
using GetWorkItems.Model.Services.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace GetWorkItems.API.Controllers {
    [Route("api/workItem")]
    [ApiController]

    public class WorkItemController : ControllerBase {
        private readonly IConfigurationService _configurationService;

        public WorkItemController() {
            _configurationService = new ConfigurationService();
        }

        [HttpGet("getworkitems")]
        public ActionResult<WorkItemReturn[]> GetWorkItems() {
            using (var db = new GetWorkItemContext()) {
                return db.WorkItems
                    .Select(w => new WorkItemReturn {
                        Id = w.Id,
                        Title = w.Title,
                        CreatedDate = w.CreatedDate,
                        WorkItemType = w.WorkItemType
                    })
                    .ToArray();
            }
        }

        [DisableCors]
        [HttpPost("saveconfiguration")]
        public ActionResult<bool> SaveConfiguration(ConfigurationDto configurationDto) {
            _configurationService.Save(configurationDto);

            return true;
        }

        [HttpGet("getconfiguration")]
        public ActionResult<ConfigurationReturn> GetConfiguration() {
            using (var db = new GetWorkItemContext()) {
                return db.Configurations
                    .Select(c => new ConfigurationReturn {
                        Url = c.Url,
                        ProjectName = c.ProjectName,
                        Token = c.Token
                    })
                    .FirstOrDefault();
            }
        }
    }
}
