using System;
using GetWorkItems.Model.Services;

namespace GetWorkItems {
    class Program {

        static void Main(string[] args) {
            try {
                var service = new WorkItemService();
                var workItemHandler = new WorkItemsHandler();

                var workItemsDto = workItemHandler.GetWorkItems();
                service.Save(workItemsDto);

                Console.Write("Work items gravados com sucesso!");
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
            }
        }
    }
}
