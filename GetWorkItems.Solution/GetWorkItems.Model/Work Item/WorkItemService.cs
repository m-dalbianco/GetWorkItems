using AutoMapper;
using GetWorkItems.Model.Dtos;
using GetWorkItems.Model.Entities;
using GetWorkItems.Model.Services.Interfaces;
using System.Linq;

namespace GetWorkItems.Model.Services {
    public class WorkItemService : IWorkItemService {
        public void Save(WorkItemDto[] worksItemsDto) {
            FillIdsWorkItems(worksItemsDto);

            foreach (var workItemDto in worksItemsDto) {
                if (workItemDto.Id == null || workItemDto.Id == 0) {
                    Include(workItemDto);
                } else {
                    Edit(workItemDto);
                }
            }
        }

        private void FillIdsWorkItems(WorkItemDto[] worksItemsDto) {
            using (var db = new GetWorkItemContext()) {
                var workItems = db.WorkItems
                    .Select(w => new {
                        w.Id,
                        w.IdWorkItem
                    })
                    .ToArray();

                foreach (var workItemDto in worksItemsDto) {
                    workItemDto.Id = workItems
                        .Where(w => w.IdWorkItem == workItemDto.IdWorkItem)
                        .Select(w => w.Id)
                        .FirstOrDefault();
                }
            }
        }

        private void Include(WorkItemDto workItemDto) {
            using (var db = new GetWorkItemContext()) {
                var newWorkItem = FillWorkItem(workItemDto);

                db.WorkItems.Add(newWorkItem);
                db.SaveChanges();
            }
        }

        private void Edit(WorkItemDto workItemDto) {
            using (var db = new GetWorkItemContext()) {
                var workItemOriginal = db.WorkItems.FirstOrDefault(w => w.Id == workItemDto.Id);
                workItemOriginal.WorkItemType = workItemDto.WorkItemType;
                workItemOriginal.Title = workItemDto.Title;
                workItemOriginal.CreatedDate = workItemDto.CreatedDate;

                db.SaveChanges();
            }
        }
        private WorkItem FillWorkItem(WorkItemDto workItemDto) {
            return new WorkItem {
                Title = workItemDto.Title,
                CreatedDate = workItemDto.CreatedDate,
                WorkItemType = workItemDto.WorkItemType,
                IdWorkItem = workItemDto.IdWorkItem.Value
            };
        }
    }
}
