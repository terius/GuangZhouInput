using BLL;
using FGTran;
using Helpers;
using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace LiHuo
{
    public partial class InputQuery : Form
    {
        private readonly InputBLL bll = new InputBLL();
        public InputQuery()
        {
            InitializeComponent();
        }
        IList<PerType> pers = GlobalVariable.LoginUserInfo.UserPer;
        private void Input_Load(object sender, EventArgs e)
        {
            labTitle.Text = GlobalVariable.LoginUserInfo.UserName + "(" + GlobalVariable.LoginUserInfo.CompanyName + ")";
            InitDataGridView();
            BindCompany();
        }

        private void InitDataGridView()
        {
            superGrid1.EnableHeadersVisualStyles = false;
            this.superGrid1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            //    this.superGrid1.AllowUserToResizeColumns = true;
            CommonHelper.AddColumn(this.superGrid1, "公司", "CompanyName");
            CommonHelper.AddColumn(this.superGrid1, "总运单", "VoyageNo");
            CommonHelper.AddColumn(this.superGrid1, "类别", "TradeType");
            CommonHelper.AddColumn(this.superGrid1, "日期", "InputDate");
            CommonHelper.AddColumn(this.superGrid1, "进出口", "I_E_FLAG");
            CommonHelper.AddColumn(this.superGrid1, "空陆运", "A_L_TYPE");
            CommonHelper.AddColumn(this.superGrid1, "流水号", "SNo");
            //CommonHelper.AddColumn(this.superGrid1, "合计票数", "AllTicket");
            //CommonHelper.AddColumn(this.superGrid1, "合计件数", "AllPiece");
            //CommonHelper.AddColumn(this.superGrid1, "合计重量", "AllWeight");
            //CommonHelper.AddColumn(this.superGrid1, "合计价值", "AllPrice");
            CommonHelper.AddColumn(this.superGrid1, "票数", "Ticket");
            CommonHelper.AddColumn(this.superGrid1, "件数", "Piece");
            CommonHelper.AddColumn(this.superGrid1, "重量", "Weight");
            CommonHelper.AddColumn(this.superGrid1, "价值", "Price");
            CommonHelper.AddColumn(this.superGrid1, "备注", "Remark");
            CommonHelper.AddHideColumn(this.superGrid1, "总运单1", "VoyageNoHide");
            CommonHelper.AddHideColumn(this.superGrid1, "公司代码", "CompanyId");
            if (GlobalVariable.LoginUserInfo.PermissonLevel == "3" || GlobalVariable.LoginUserInfo.PermissonLevel == "4")
            {
                CommonHelper.AddColumn(this.superGrid1, "统计票数", "Ticket_C");
                CommonHelper.AddColumn(this.superGrid1, "统计件数", "Piece_C");
                CommonHelper.AddColumn(this.superGrid1, "统计重量", "Weight_C");
                CommonHelper.AddColumn(this.superGrid1, "统计价值", "Price_C");
                DataGridViewButtonColumn columnButton = new DataGridViewButtonColumn();
                columnButton.Name = columnButton.DataPropertyName = "DUIPENG";
                columnButton.FillWeight = 70F;
                columnButton.HeaderText = "重新对碰";
                columnButton.Text = "重新对碰";
                columnButton.UseColumnTextForButtonValue = true;
                this.superGrid1.Columns.Add(columnButton);
            }
        }

        private void BindCompany()
        {
            DataTable dt = bll.GetCompanyList();
            DataRow dr = dt.NewRow();
            dr[0] = "";
            dr[1] = "-----全部----";
            dt.Rows.InsertAt(dr, 0);
            cboxCompany.DataSource = dt;
            cboxCompany.DisplayMember = "CompanyName";
            cboxCompany.ValueMember = "CompanyId";

            if (pers.Contains(PerType.QueryCompany))
            {
                cboxCompany.SelectedValue = GlobalVariable.LoginUserInfo.CompanyId;
                cboxCompany.Enabled = false;
            }
        }




        private void btnQuery_Click(object sender, EventArgs e)
        {
            QueryData();
        }

        private void QueryData()
        {
            string voyage = txtVoyageNo.Text.Trim();
            string companyId = cboxCompany.SelectedValue.ToString();
            DateTime sDate = DateTime.MinValue;
            if (stime.Checked)
            {
                sDate = stime.Value.Date;
            }
            DateTime eDate = DateTime.MinValue;
            if (etime.Checked)
            {
                eDate = etime.Value.Date;
            }

            string ieFlag = "";
            if (rbI.Checked)
            {
                ieFlag = "I";
            }
            else if (rbE.Checked)
            {
                ieFlag = "E";
            }
            DataTable dt = bll.QueryInputWork(voyage, companyId, sDate, eDate, ieFlag, GlobalVariable.LoginUserInfo);
            this.superGrid1.DataSource = dt;
            ResizeColumns();
            SetButtonEnable();
        }

        private void SetButtonEnable()
        {
            string stype = "";
            foreach (DataGridViewRow row in this.superGrid1.Rows)
            {
                stype = row.Cells["TradeType"].Value.ToString();
                if (stype == "普货" || stype == "合计")
                {
                    DataGridViewTextBoxCell txtcell = new DataGridViewTextBoxCell();
                    row.Cells["DUIPENG"] = txtcell;
                    row.Cells["DUIPENG"].ReadOnly = true;
                    //row.Cells["DUIPENG"].
                    //  disButtonCell = (DataGridViewDisableButtonCell)row.Cells["DUIPENG"];

                    //    disButtonCell.Value = "111";
                    // (row.Cells["DUIPENG"]).Value = "1111";
                    //    disButtonCell.Enabled = false;
                    //   disButtonCell.Value = "";
                    //   ((DataGridViewDisableButtonCell)row.Cells["DUIPENG"]).Enabled = false;
                }
            }
            superGrid1.Refresh();
        }

        private void ResizeColumns()
        {
            int width = 0;
            foreach (DataGridViewColumn c in this.superGrid1.Columns)
            {
                width = c.Width;
                c.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                c.Width = width;
            }
        }

        private void superGrid1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (this.superGrid1.Rows[e.RowIndex].Cells["TradeType"].Value.ToString() == "合计")
            {
                e.CellStyle.BackColor = Color.FromArgb(255, 239, 213);

            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (this.superGrid1.Rows.Count > 0)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "导出Excel (*.xls)|*.xls";
                saveFileDialog.FilterIndex = 0;
                saveFileDialog.RestoreDirectory = true;
                //   saveFileDialog.CreatePrompt = true;
                saveFileDialog.Title = "导出文件保存路径";
                saveFileDialog.FileName = "查询_" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;
                    if (filePath.Length != 0)
                    {
                        labMessage.Text = "正在导出Excel";
                        DataTable dt = (DataTable)this.superGrid1.DataSource; // Se convierte el DataSource 
                        Hashtable ht = CommonHelper.GetDataGridViewDisplayColumns(this.superGrid1);
                        ExcelHelper.Export(dt, filePath, ht);
                        labMessage.Text = "";
                        MessageBox.Show("导出成功");
                    }
                }
            }
        }


        private void superGrid1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            if (pers.Contains(PerType.EditALL) || pers.Contains(PerType.EditCompany))
            {
                if (e.ColumnIndex >= 0 && e.RowIndex >= 0 && senderGrid.Columns[e.ColumnIndex] is DataGridViewTextBoxColumn)
                {
                    string companyId = this.superGrid1.Rows[e.RowIndex].Cells["CompanyId"].Value.ToString();
                    if (pers.Contains(PerType.EditCompany) && GlobalVariable.LoginUserInfo.CompanyId != companyId)
                    {
                        MessageBox.Show("您无权修改其他公司的总运单信息");
                        return;
                    }
                    string voyage = this.superGrid1.Rows[e.RowIndex].Cells["VoyageNoHide"].Value.ToString();

                    if (!string.IsNullOrEmpty(voyage))
                    {
                        bool isA = this.superGrid1.Rows[e.RowIndex].Cells["A_L_TYPE"].Value.ToString() == "空运" ? true : false;
                        Input frm = new Input(voyage, isA, companyId);
                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            MessageBox.Show("修改成功");
                            QueryData();
                        }
                    }
                }
            }
        }

        private void superGrid1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (e.ColumnIndex >= 0 && e.RowIndex >= 0 && senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
            {
                string val = senderGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                if (val == "重新对碰")
                {
                    DataGridViewRow dr = senderGrid.Rows[e.RowIndex];
                    DuiPeng(dr);
                }


                //DataGridViewDisableButtonCell buttonCell = (DataGridViewDisableButtonCell)senderGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                //if (buttonCell.Enabled)
                //{
                //    DataGridViewRow dr = senderGrid.Rows[e.RowIndex];
                //    DuiPeng(dr);
                //}
            }
        }

        private void DuiPeng(DataGridViewRow dr)
        {
            string voyageNo = dr.Cells["VoyageNo"].Value.ToString();
            string tradeType = dr.Cells["TradeType"].Value.ToString();
            string comId = dr.Cells["CompanyId"].Value.ToString();
            V1Info info = bll.DuiPeng(voyageNo, tradeType, comId);
            dr.Cells["Ticket_C"].Value = info.Ticket;
            dr.Cells["Piece_C"].Value = info.PackNo;
            dr.Cells["Weight_C"].Value = info.Weight;
            MessageBox.Show("对碰成功");
        }
    }
}
