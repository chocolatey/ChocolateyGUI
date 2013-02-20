using System;
using System.ComponentModel;
using System.Windows.Forms;
using Chocolatey.Explorer.Model;
using Chocolatey.Explorer.Services.PackageVersionService;
using StructureMap;

namespace Chocolatey.Explorer.View.Controls
{
    public class PackagesBaseGrid : DataGridView
    {
        protected bool IsLoading;

        public PackagesBaseGrid()
        {
            ObjectFactory.BuildUp(this);
        }

        private  IPackageVersionService _packageVersionService;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IPackageVersionService PackageVersionService
        {
            get { return _packageVersionService; }
            set
            {
                _packageVersionService = value;
                _packageVersionService.VersionChanged += VersionChanged;
                DoLayout();
            }
        }

        private void VersionChanged(PackageVersion version)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => VersionChanged(version)));
            }
            else
            {
                Enabled = true;
                IsLoading = false;
            }
        }

        protected void GridSelectionChanged(object sender, EventArgs e)
        {
            if (SelectedRows.Count <= 0 || IsLoading) return;
            Enabled = false;
            IsLoading = true;
            _packageVersionService.PackageVersion(SelectedRows[0].Cells[0].Value.ToString());
        }

        private void DoLayout()
        {
            SelectionChanged += GridSelectionChanged;
            RowHeadersVisible = false;
            MultiSelect = false;
            SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            AccessibleRole = AccessibleRole.List;
            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;
            var dataGridViewCellStyle1 = new DataGridViewCellStyle { BackColor = System.Drawing.SystemColors.Control };
            AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            BackgroundColor = System.Drawing.SystemColors.Control;
            BorderStyle = BorderStyle.None;
            CellBorderStyle = DataGridViewCellBorderStyle.None;
            var dataGridViewCellStyle2 = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                BackColor = System.Drawing.SystemColors.Control,
                Font =
                    new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular,
                                            System.Drawing.GraphicsUnit.Point, 0),
                ForeColor = System.Drawing.SystemColors.WindowText,
                SelectionBackColor = System.Drawing.SystemColors.Highlight,
                SelectionForeColor = System.Drawing.SystemColors.HighlightText,
                WrapMode = DataGridViewTriState.True
            };
            ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            EditMode = DataGridViewEditMode.EditProgrammatically;
            GridColor = System.Drawing.SystemColors.Control;
            Name = "PackageGrid";
            ReadOnly = true;
            RowTemplate.ReadOnly = true;
            RowTemplate.Resizable = DataGridViewTriState.False;
            ShowCellErrors = false;
            ShowCellToolTips = false;
            ShowEditingIcon = false;
            ShowRowErrors = false;
            StandardTab = true;
            var column1 = new DataGridViewCheckBoxColumn
            {
                DataPropertyName = "IsInstalled",
                HeaderText = "Installed",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
            };
            var column2 = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Name",
                HeaderText = "Name",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };
            var column3 = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "InstalledVersion",
                HeaderText = "Installed version",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            };
            var column4 = new DataGridViewCheckBoxColumn
            {
                DataPropertyName = "IsPreRelease",
                HeaderText = "Pre release",
                ReadOnly = true,
                Width = 70
            };
            if (ColumnCount == 0)
            {
                Columns.Add(column1);
                Columns.Add(column2);
                Columns.Add(column3);
                Columns.Add(column4);
            }
        }
    }
}