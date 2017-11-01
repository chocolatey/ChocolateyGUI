using System.Collections;
using System.ComponentModel;

namespace ChocolateyGui.Utilities
{
    public interface IDataGridColumnComparer : IComparer
    {
        ListSortDirection SortDirection { get; set; }
    }
}