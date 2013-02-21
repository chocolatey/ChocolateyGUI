using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Chocolatey.Explorer.Model;
using Chocolatey.Explorer.Services.PackageVersionService;
using StructureMap;

namespace Chocolatey.Explorer.View.Controls
{
    public class PackagesBaseGrid : UserControl
    {
        protected bool IsLoading;
        private DataGridView packagesGrid;
        private Panel panel1;
        private TextBox txtSearch;
        private Label label2;
        protected BindingSource _bindingsource;

        public PackagesBaseGrid()
        {
            _bindingsource = new BindingSource();
            ObjectFactory.BuildUp(this);
            InitializeComponent();
            DoLayout();
            packagesGrid.DataSource = _bindingsource;
            txtSearch.TextChanged += TxtSearchTextChanged;
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
            }
        }

        public int Rowcount { get { return packagesGrid.RowCount; } }

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
            if (packagesGrid.SelectedRows.Count <= 0 || IsLoading) return;
            Enabled = false;
            IsLoading = true;
            _packageVersionService.PackageVersion(packagesGrid.SelectedRows[0].Cells[0].Value.ToString());
        }

        private void DoLayout()
        {
            packagesGrid.SelectionChanged += GridSelectionChanged;
            packagesGrid.RowHeadersVisible = false;
            packagesGrid.MultiSelect = false;
            packagesGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            packagesGrid.AccessibleRole = AccessibleRole.List;
            packagesGrid.AllowUserToAddRows = false;
            packagesGrid.AllowUserToDeleteRows = false;
            var dataGridViewCellStyle1 = new DataGridViewCellStyle { BackColor = System.Drawing.SystemColors.Control };
            packagesGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            packagesGrid.BackgroundColor = System.Drawing.SystemColors.Control;
            packagesGrid.BorderStyle = BorderStyle.None;
            packagesGrid.CellBorderStyle = DataGridViewCellBorderStyle.None;
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
            packagesGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            packagesGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            packagesGrid.EditMode = DataGridViewEditMode.EditProgrammatically;
            packagesGrid.GridColor = System.Drawing.SystemColors.Control;
            packagesGrid.Name = "PackageGrid";
            packagesGrid.ReadOnly = true;
            packagesGrid.RowTemplate.ReadOnly = true;
            packagesGrid.RowTemplate.Resizable = DataGridViewTriState.False;
            packagesGrid.ShowCellErrors = false;
            packagesGrid.ShowCellToolTips = false;
            packagesGrid.ShowEditingIcon = false;
            packagesGrid.ShowRowErrors = false;
            packagesGrid.StandardTab = true;
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
            if (packagesGrid.ColumnCount != 0) return;
            packagesGrid.Columns.Add(column1);
            packagesGrid.Columns.Add(column2);
            packagesGrid.Columns.Add(column3);
            packagesGrid.Columns.Add(column4);
        }

        private void TxtSearchTextChanged(object sender, EventArgs e)
        {
            DataGridViewRow rowFound = packagesGrid.Rows.OfType<DataGridViewRow>()
                    .FirstOrDefault(row => row.Cells.OfType<DataGridViewCell>()
                    .Any(cell => cell.ColumnIndex == 0 && ((String)cell.Value).StartsWith(txtSearch.Text, StringComparison.OrdinalIgnoreCase)));

            if (rowFound != null)
            {
                selectPackageGridRow(rowFound.Index);
            }
        }

        private void selectPackageGridRow(int rowIndex)
        {
            packagesGrid.Rows[rowIndex].Selected = true;
            packagesGrid.FirstDisplayedScrollingRowIndex = rowIndex;
            packagesGrid.CurrentCell = packagesGrid.Rows[rowIndex].Cells[0];
        }

        private void InitializeComponent()
        {
            this.packagesGrid = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.packagesGrid)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // packagesGrid
            // 
            this.packagesGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.packagesGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.packagesGrid.Location = new System.Drawing.Point(0, 33);
            this.packagesGrid.Name = "packagesGrid";
            this.packagesGrid.Size = new System.Drawing.Size(296, 399);
            this.packagesGrid.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtSearch);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(296, 33);
            this.panel1.TabIndex = 4;
            // 
            // txtSearch
            // 
            this.txtSearch.AccessibleDescription = "Search for packages";
            this.txtSearch.AccessibleName = "Search";
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.Location = new System.Drawing.Point(69, 4);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(212, 20);
            this.txtSearch.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AccessibleDescription = "Search for packages";
            this.label2.AccessibleName = "Search";
            this.label2.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
            this.label2.AutoSize = true;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(4, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "&Search";
            // 
            // PackagesBaseGrid
            // 
            this.Controls.Add(this.packagesGrid);
            this.Controls.Add(this.panel1);
            this.Name = "PackagesBaseGrid";
            this.Size = new System.Drawing.Size(296, 432);
            ((System.ComponentModel.ISupportInitialize)(this.packagesGrid)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}