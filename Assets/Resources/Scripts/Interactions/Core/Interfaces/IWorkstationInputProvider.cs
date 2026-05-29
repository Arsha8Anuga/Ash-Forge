using System.Collections.Generic;

public interface IWorkstationInputProvider
{
    List<PhysicalItem> GetItems();
}