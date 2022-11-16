using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;
using System.Net.Http;

namespace SLIC_Services_New
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        string myCon = ConfigurationManager.ConnectionStrings["dbCon"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnCheckValidity_Click(object sender, EventArgs e)
        {
            string myQuery = "select *  from MCOMP.MCACTUPDET where pdrefer = :Policynumber and pdnic = :NIC ";

            OracleConnection con = new OracleConnection(myCon);
            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = myQuery;
            cmd.Connection = con;

            cmd.Parameters.AddWithValue(":Policynumber", txtPolicyNumber.Text.Trim());
            cmd.Parameters.AddWithValue(":NIC", txtNIC.Text.Trim());

            OracleDataAdapter dadapter = new OracleDataAdapter();
            dadapter.SelectCommand = cmd;
            DataTable dtable = new DataTable();
            dadapter.Fill(dtable);


            if (dtable.Rows.Count > 0)
            {
                lblMessage.Text = "Details Found !";


            }
            else
            {
                lblMessage.Text = "Not Found";

            }
            }

        }
    }
