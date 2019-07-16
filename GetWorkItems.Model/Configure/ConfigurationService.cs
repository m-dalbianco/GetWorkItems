using AutoMapper;
using GetWorkItems.Model.Dtos;
using GetWorkItems.Model.Entities;
using GetWorkItems.Model.Services.Interfaces;
using System.Linq;

namespace GetWorkItems.Model.Services {
    public class ConfigurationService : IConfigurationService {
        private readonly IMapper _mapper;

        public void Save(ConfigurationDto configurationDto) {
            if (!CheckConfiguration()) {
                Include(configurationDto);
            } else {
                Edit(configurationDto);
            }
        }
   
        private bool CheckConfiguration() {
            using (var db = new GetWorkItemContext()) {
                return db.Configurations.FirstOrDefault() != null;
            }
        }

        private void Include(ConfigurationDto configurationDto) {
            using (var db = new GetWorkItemContext()) {
                var newConfiguration = FillConfiguration(configurationDto);

                db.Configurations.Add(newConfiguration);
                db.SaveChanges();
            }
        }

        private void Edit(ConfigurationDto configurationDto) {
            using (var db = new GetWorkItemContext()) {
                var configurationOriginal = db.Configurations.FirstOrDefault();
                configurationOriginal.Url = configurationDto.Url;
                configurationOriginal.Token = configurationDto.Token;
                configurationOriginal.ProjectName = configurationDto.ProjectName;

                db.SaveChanges();
            }
        }

        private Configuration FillConfiguration(ConfigurationDto configurationDto) {
            return new Configuration {
                ProjectName = configurationDto.ProjectName,
                Token = configurationDto.Token,
                Url = configurationDto.Url
            };
        }
    }
}
