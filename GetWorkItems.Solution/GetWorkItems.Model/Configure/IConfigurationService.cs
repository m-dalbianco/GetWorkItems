using GetWorkItems.Model.Dtos;

namespace GetWorkItems.Model.Services.Interfaces {
    public interface IConfigurationService {
        void Save(ConfigurationDto configurationDto);
    }
}