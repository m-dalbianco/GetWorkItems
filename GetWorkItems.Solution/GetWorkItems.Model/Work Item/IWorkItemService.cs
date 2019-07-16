using GetWorkItems.Model.Dtos;

namespace GetWorkItems.Model.Services.Interfaces {
    public interface IWorkItemService {
        void Save(WorkItemDto[] worksItemsDto);
    }
}
