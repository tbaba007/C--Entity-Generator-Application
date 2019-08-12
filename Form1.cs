using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
namespace EntityGenerator
{
    public partial class Form1 : Form
    {
        SqlDataReader rdr;
        string tablename;
        string fkname;
        List<string> tables = new List<string>();

        string newi = null;
        List<string> loopeditems = new List<string>();
        List<check> list = new List<check>();
        string loop;
        string pk;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {

            try
            {


                if (string.IsNullOrEmpty(txtConnectionString.Text.Trim()))
                {
                    MessageBox.Show("Please Enter A Connection String", "EntityGenerator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (string.IsNullOrEmpty(txtprojectname.Text.Trim()))
                {
                    MessageBox.Show("Please Enter A Project Name", "EntityGenerator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (string.IsNullOrEmpty(txtscriptoutput.Text.Trim()))
                {
                    MessageBox.Show("Please Enter A Script Output", "EntityGenerator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (checkedListBox1.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Please Select A Valid Database", "EntityGenerator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (cmboutput.Text == "---Select---")
                {
                    MessageBox.Show("Please Select A Valid output Type", "EntityGenerator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (string.IsNullOrEmpty(txtEntitiesName.Text))
                {
                    MessageBox.Show("Please Enter Entities Name", "EntityGenerator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (!txtEntitiesName.Text.Contains("Entities"))
                {
                    MessageBox.Show("Please Enter A Valid Entity Name. (Entities) must be appended to the name.", "EntityGenerator");
                    return;
                }
                if (string.IsNullOrEmpty(txtprojectname.Text))
                {
                    MessageBox.Show("Please Enter ProjectName", "EntityGenerator", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }

                //############ load all databases in the selected server #############

                LoadAllTables();

                //############ Remove Sysdiagrams #############

                if (tables.Contains("sysdiagrams"))
                {
                    tables.Remove("sysdiagrams");
                }
                foreach (var tbl in tables)
                {
                    var outputpath = txtscriptoutput.Text;
                    var newpath = outputpath.Replace("\"", "\\");

                    //############ Create A Path #############


                    var newestpath = Path.Combine(newpath, txtprojectname.Text);

                    var Datacontracts = Path.Combine(newestpath + "\\" + txtprojectname.Text + ".DataContracts");

                    Directory.CreateDirectory(Datacontracts);
                    if (cmboutput.Text == "Android")
                    {
                        Datacontracts = Datacontracts + "\\" + tbl + ".Java";
                    }
                    else
                    {
                        Datacontracts = Datacontracts + "\\" + tbl + "Contract" + ".cs";
                    }


                    txtactivity.AppendText(Environment.NewLine);
                    txtactivity.AppendText("Scripting Table" + " " + tbl + " " + "........................");
                    txtactivity.AppendText(Environment.NewLine);
                    txtactivity.AppendText(Environment.NewLine);


                    using (Stream str = File.Create(Datacontracts))
                    {
                        if (cmboutput.Text == "Aspx")
                        {
                            WriteToFileASP(tbl);

                        }
                        if (cmboutput.Text == "Mvc")
                        {
                            WriteToFileMVC(tbl);
                        }
                        if (cmboutput.Text == "Android")
                        {
                            WriteToFileAndroid(tbl);
                            GenerateAndroid2(tbl);
                        }

                        TextWriter tw = new StreamWriter(str);
                        tw.Write(txtoutput.Text);
                        txtoutput.Text = "";
                        tw.Close();

                    }

                    txtactivity.AppendText("Scripting Table" + " " + tbl + " " + "completed");
                    txtactivity.AppendText(Environment.NewLine);
                }

                //*********************************************************************************************************************************
                //Method Below is to Generate Customized Models
                //*********************************************************************************************************************************

                foreach (var tbl in tables)
                {
                    loop = tbl;
                    var outputpath = txtscriptoutput.Text;
                    var newpath = outputpath.Replace("\"", "\\");

                    var newestpath = Path.Combine(newpath, txtprojectname.Text);
                    var customizedpath = Path.Combine(newestpath + "\\" + txtprojectname.Text + ".DataContracts" + "\\" + "Customized" + txtprojectname.Text + "DataContracts");



                    Directory.CreateDirectory(customizedpath);
                    if (cmboutput.Text == "Android")
                    {
                        customizedpath = customizedpath + "\\" + "Customized" + tbl + ".Java";
                    }
                    else
                    {
                        customizedpath = customizedpath + "\\" + "Customized" + tbl + "Contract" + ".cs";
                    }

                    var scc = customizedpath + "\\" + tbl + ".cs";
                    txtactivity.AppendText(Environment.NewLine);
                    txtactivity.AppendText("Scripting Table" + " " + tbl + " " + "........................");
                    txtactivity.AppendText(Environment.NewLine);
                    txtactivity.AppendText(Environment.NewLine);


                    using (Stream str = File.Create(customizedpath))
                    {
                        if (cmboutput.Text == "Aspx")
                        {

                            CustomizedDataContracts(tbl);
                        }
                        if (cmboutput.Text == "Mvc")
                        {

                            CustomizedDataContracts(tbl);
                        }
                        if (cmboutput.Text == "Android")
                        {
                            WriteToFileAndroid(tbl);
                            GenerateAndroid2(tbl);
                        }

                        TextWriter tw = new StreamWriter(str);
                        tw.Write(txtoutput.Text);
                        txtoutput.Text = "";
                        tw.Close();

                    }


                    txtactivity.AppendText("Scripting Customized Table" + " " + tbl + " " + "completed");
                    txtactivity.AppendText(Environment.NewLine);
                }



                //*********************************************************************************************************************************
                //Method Below is to Generate Services
                //*********************************************************************************************************************************

                foreach (var tbl in tables)
                {
                    var outputpath = txtscriptoutput.Text;
                    var newpath = outputpath.Replace("\"", "\\");

                    var newestpath = Path.Combine(newpath, txtprojectname.Text);
                    var ServicePath = Path.Combine(newestpath + "\\" + txtprojectname.Text + "." + "Services");


                    Directory.CreateDirectory(ServicePath);
                    if (cmboutput.Text == "Android")
                    {
                        ServicePath = ServicePath + "\\" + tbl + ".Java";
                    }
                    else
                    {
                        ServicePath = ServicePath + "\\" + tbl + "Services" + ".cs";
                    }


                    txtactivity.AppendText(Environment.NewLine);
                    txtactivity.AppendText("Scripting Table" + " " + tbl + " " + "........................");
                    txtactivity.AppendText(Environment.NewLine);
                    txtactivity.AppendText(Environment.NewLine);


                    using (Stream str = File.Create(ServicePath))
                    {
                        if (cmboutput.Text == "Aspx")
                        {

                            GenerateServiceContracts(tbl);
                        }
                        if (cmboutput.Text == "Mvc")
                        {

                            GenerateServiceContracts(tbl);
                        }
                        if (cmboutput.Text == "Android")
                        {
                            WriteToFileAndroid(tbl);
                            GenerateAndroid2(tbl);
                        }

                        TextWriter tw = new StreamWriter(str);
                        tw.Write(txtoutput.Text);
                        txtoutput.Text = "";
                        tw.Close();

                    }
                }



                //*********************************************************************************************************************************
                //Method Below is to Generate Helpers(Session Helper etc)
                //*********************************************************************************************************************************


                var _outputpath = txtscriptoutput.Text;
                var _newpath = _outputpath.Replace("\"", "\\");

                var _newestpath = Path.Combine(_newpath, txtprojectname.Text);
                var _ServicePath = Path.Combine(_newestpath + "\\" + txtprojectname.Text + "." + "Helpers");


                Directory.CreateDirectory(_ServicePath);

                _ServicePath = _ServicePath + "\\" + txtprojectname.Text.Trim() + "Helpers" + ".cs";



                txtactivity.AppendText(Environment.NewLine);
                txtactivity.AppendText("Scripting Helper  for" + " " + txtprojectname.Text.Trim() + " " + "........................");
                txtactivity.AppendText(Environment.NewLine);
                txtactivity.AppendText(Environment.NewLine);


                using (Stream str = File.Create(_ServicePath))
                {
                    GenerateHelpers();

                    TextWriter tw = new StreamWriter(str);
                    tw.Write(txtoutput.Text);
                    txtoutput.Text = "";
                    tw.Close();

                }





                //*********************************************************************

                //Generate Customized Services

                //*********************************************************************
                foreach (var tbl in tables)
                {
                    var outputpath = txtscriptoutput.Text;
                    var newpath = outputpath.Replace("\"", "\\");

                    var newestpath = Path.Combine(newpath, txtprojectname.Text);
                    var ServicePathcustomized = Path.Combine(newestpath + "\\" + txtprojectname.Text + "." + "Services" + "\\" + "CustomizedServices");


                    Directory.CreateDirectory(ServicePathcustomized);
                    if (cmboutput.Text == "Android")
                    {
                        ServicePathcustomized = ServicePathcustomized + "\\" + "Customized" + tbl + ".Java";
                    }
                    else
                    {
                        ServicePathcustomized = ServicePathcustomized + "\\" + "Customized" + tbl + "Services" + ".cs";
                    }


                    txtactivity.AppendText(Environment.NewLine);
                    txtactivity.AppendText("Scripting Table" + " " + tbl + " " + "........................");
                    txtactivity.AppendText(Environment.NewLine);
                    txtactivity.AppendText(Environment.NewLine);


                    using (Stream str = File.Create(ServicePathcustomized))
                    {
                        if (cmboutput.Text == "Aspx")
                        {


                            GenerateCustomizedServiceContracts(tbl);
                        }
                        if (cmboutput.Text == "Mvc")
                        {

                            GenerateCustomizedServiceContracts(tbl);
                        }
                        if (cmboutput.Text == "Android")
                        {
                            WriteToFileAndroid(tbl);
                            GenerateAndroid2(tbl);
                        }

                        TextWriter tw = new StreamWriter(str);
                        tw.Write(txtoutput.Text);
                        txtoutput.Text = "";
                        tw.Close();

                    }
                }


                //*********************************************************************************************************************************
                //Method Below is to Generate ServiceContracts in a folder
                //*********************************************************************************************************************************

                foreach (var tbl in tables)
                {

                    newi = tbl;
                    var outputpath = txtscriptoutput.Text;
                    var newpath = outputpath.Replace("\"", "\\");

                    var newestpath = Path.Combine(newpath, txtprojectname.Text);
                    var ServiceContractsPath = Path.Combine(newestpath + "\\" + txtprojectname.Text + "." + "Services" + "\\" + "ServiceContracts");


                    Directory.CreateDirectory(ServiceContractsPath);
                    if (cmboutput.Text == "Android")
                    {
                        ServiceContractsPath = ServiceContractsPath + "\\" + tbl + ".Java";
                    }
                    else
                    {
                        ServiceContractsPath = ServiceContractsPath + "\\" + "Iserviceprovider.cs";
                    }


                    txtactivity.AppendText(Environment.NewLine);
                    txtactivity.AppendText("Scripting Table" + " " + tbl + " " + "........................");
                    txtactivity.AppendText(Environment.NewLine);
                    txtactivity.AppendText(Environment.NewLine);


                    using (Stream str = File.Create(ServiceContractsPath))
                    {
                        if (cmboutput.Text == "Aspx")
                        {
                            // WriteToFileASP(tbl);
                            GenerateServiceContractInterface();
                        }
                        if (cmboutput.Text == "Mvc")
                        {
                            // WriteToFileMVC(tbl);
                            GenerateServiceContractInterface();
                        }
                        if (cmboutput.Text == "Android")
                        {
                            WriteToFileAndroid(tbl);
                            GenerateAndroid2(tbl);
                        }

                        TextWriter tw = new StreamWriter(str);
                        tw.Write(txtoutput.Text);
                        txtoutput.Text = "";
                        tw.Close();

                    }

                    //*********************************************************************************************************************************
                    //Method Below is to Generate ServiceContracts Standard Service Provider in a folder
                    //*********************************************************************************************************************************
                }
                foreach (var tbl in tables)
                {

                    newi = tbl;
                    var outputpath = txtscriptoutput.Text;
                    var newpath = outputpath.Replace("\"", "\\");

                    var newestpath = Path.Combine(newpath, txtprojectname.Text);
                    var ServiceContractsPath = Path.Combine(newestpath + "\\" + txtprojectname.Text + "." + "Services" + "\\" + "ServiceContracts");


                    Directory.CreateDirectory(ServiceContractsPath);
                    if (cmboutput.Text == "Android")
                    {
                        ServiceContractsPath = ServiceContractsPath + "\\" + tbl + ".Java";
                    }
                    else
                    {
                        ServiceContractsPath = ServiceContractsPath + "\\" + "StandardServiceProvider.cs";
                    }

                    txtactivity.AppendText(Environment.NewLine);
                    txtactivity.AppendText("Scripting Table" + " " + tbl + " " + "........................");
                    txtactivity.AppendText(Environment.NewLine);
                    txtactivity.AppendText(Environment.NewLine);


                    using (Stream str = File.Create(ServiceContractsPath))
                    {
                        if (cmboutput.Text == "Aspx")
                        {
                            // WriteToFileASP(tbl);
                            GenerateServiceContractStandardServiceProvider();
                        }
                        if (cmboutput.Text == "Mvc")
                        {
                            // WriteToFileMVC(tbl);
                            GenerateServiceContractStandardServiceProvider();
                        }
                        if (cmboutput.Text == "Android")
                        {
                            WriteToFileAndroid(tbl);
                            GenerateAndroid2(tbl);
                        }

                        TextWriter tw = new StreamWriter(str);
                        tw.Write(txtoutput.Text);
                        txtoutput.Text = "";
                        tw.Close();

                    }

                    txtactivity.AppendText("Scripting Customized Table" + " " + tbl + " " + "completed");
                    txtactivity.AppendText(Environment.NewLine);
                }

                foreach (var tbl in tables)
                {

                    newi = tbl;
                    var outputpath = txtscriptoutput.Text;
                    var newpath = outputpath.Replace("\"", "\\");

                    var newestpath = Path.Combine(newpath, txtprojectname.Text);
                    var ServiceContractsPath = Path.Combine(newestpath + "\\" + txtprojectname.Text + "." + "Services" + "\\" + "ServiceContracts");


                    Directory.CreateDirectory(ServiceContractsPath);
                    if (cmboutput.Text == "Android")
                    {
                        ServiceContractsPath = ServiceContractsPath + "\\" + tbl + ".Java";
                    }
                    else
                    {
                        ServiceContractsPath = ServiceContractsPath + "\\" + "ServiceProvider.cs";
                    }

                    txtactivity.AppendText(Environment.NewLine);
                    txtactivity.AppendText("Scripting Table" + " " + tbl + " " + "........................");
                    txtactivity.AppendText(Environment.NewLine);
                    txtactivity.AppendText(Environment.NewLine);


                    using (Stream str = File.Create(ServiceContractsPath))
                    {
                        if (cmboutput.Text == "Aspx")
                        {
                            // WriteToFileASP(tbl);
                            GenerateServiceContractServiceProvider();
                        }
                        if (cmboutput.Text == "Mvc")
                        {
                            // WriteToFileMVC(tbl);
                            GenerateServiceContractServiceProvider();
                        }
                        if (cmboutput.Text == "Android")
                        {
                            WriteToFileAndroid(tbl);
                            GenerateAndroid2(tbl);
                        }

                        TextWriter tw = new StreamWriter(str);
                        tw.Write(txtoutput.Text);
                        txtoutput.Text = "";
                        tw.Close();

                    }

                    txtactivity.AppendText("Scripting Customized Table" + " " + tbl + " " + "completed");
                    txtactivity.AppendText(Environment.NewLine);
                }




                //*********************************************************************

                //Generate Infrastructure 

                //*********************************************************************

                var path = txtscriptoutput.Text;
                var npath = path.Replace("\"", "\\");

                var nestpath = Path.Combine(npath, txtprojectname.Text);
                var Infrastructure = Path.Combine(nestpath + "\\" + txtprojectname.Text + "." + "Infrastructure");
                //var Uow = Path.Combine(Infrastructure + "." + "Uow");

                Directory.CreateDirectory(Infrastructure);

                Infrastructure = Infrastructure + "\\" + "IRepository.cs";
                // var Uow = Infrastructure + "\\" + "Uow.cs";

                txtactivity.AppendText(Environment.NewLine);

                txtactivity.AppendText(Environment.NewLine);
                txtactivity.AppendText(Environment.NewLine);


                using (Stream str = File.Create(Infrastructure))
                {
                    if (cmboutput.Text == "Aspx")
                    {

                        GenerateInfrastructure();
                    }
                    if (cmboutput.Text == "Mvc")
                    {

                        GenerateInfrastructure();
                    }


                    TextWriter tw = new StreamWriter(str);
                    tw.Write(txtoutput.Text);
                    txtoutput.Text = "";


                    //  Infrastructure = "";
                    // Infrastructure = Infrastructure + "\\" + "uow.cs";

                    tw.Close();
                }





                Infrastructure = Infrastructure.Replace("IRepository", "uow"); //+"\\" + "IRepository.cs";


                using (Stream strr = File.Create(Infrastructure))
                {
                    if (cmboutput.Text == "Aspx")
                    {

                        GenerateUnitofwork();
                    }
                    if (cmboutput.Text == "Mvc")
                    {

                        GenerateUnitofwork();
                    }


                    TextWriter tww = new StreamWriter(strr);
                    tww.Write(txtoutput.Text);
                    txtoutput.Text = "";

                    tww.Close();



                }




                //*********************************************************************

                //Generate  Repositories

                //*********************************************************************
                foreach (var tbll in tables)
                {

                    var pathe = txtscriptoutput.Text;
                    var npathe = path.Replace("\"", "\\");

                    var npa = Path.Combine(npathe, txtprojectname.Text);
                    var Repositories = Path.Combine(npa + "\\" + txtprojectname.Text + "." + "Repositories");


                    Directory.CreateDirectory(Repositories);
                    if (cmboutput.Text == "Aspx")
                    {
                        Repositories = Repositories + "\\" + tbll + "Repository.cs";
                        GenerateRepositories(tbll);
                        // GenerateCustomizedRepositories(tbll);
                    }
                    if (cmboutput.Text == "Mvc")
                    {
                        Repositories = Repositories + "\\" + tbll + "Repository.cs";
                        GenerateRepositories(tbll);
                        //  GenerateCustomizedRepositories(tbll);

                    }

                    txtactivity.AppendText(Environment.NewLine);

                    txtactivity.AppendText(Environment.NewLine);
                    txtactivity.AppendText(Environment.NewLine);


                    using (Stream str = File.Create(Repositories))
                    {

                        TextWriter tw = new StreamWriter(str);
                        tw.Write(txtoutput.Text);
                        txtoutput.Text = "";
                        tw.Close();

                    }
                    if (tbll == null)
                    {
                        break;
                    }

                }

                //**********************************************************************

                //This method is to generate Customized Repositories

                //**********************************************************************



                //foreach (var tbll in tables)
                //{

                //    var pathe = txtscriptoutput.Text;
                //    var npathe = path.Replace("\"", "\\");

                //    var npa = Path.Combine(npathe, txtprojectname.Text);
                //    var Repositories1 = Path.Combine(npa + "\\" + txtprojectname.Text + "." + "Repositories" + "\\" + "CustomizedRepositories");
                //    //     var ServicePathcustomized = Path.Combine(newestpath + "\\" + txtprojectname.Text + "." + "Services" + "\\" + "CustomizedServices");

                //    Directory.CreateDirectory(Repositories1);
                //    if (cmboutput.Text == "Aspx")
                //    {
                //        Repositories1 = Repositories1 + "\\" + tbll + "Repository.cs";
                //        // GenerateRepositories(tbll);
                //        GenerateCustomizedRepositories(tbll);
                //    }
                //    if (cmboutput.Text == "Mvc")
                //    {
                //        Repositories1 = Repositories1 + "\\" + tbll + "Repository.cs";
                //        // GenerateRepositories(tbll);
                //        GenerateCustomizedRepositories(tbll);

                //    }

                //    txtactivity.AppendText(Environment.NewLine);

                //    txtactivity.AppendText(Environment.NewLine);
                //    txtactivity.AppendText(Environment.NewLine);


                //    using (Stream str = File.Create(Repositories1))
                //    {

                //        TextWriter tw = new StreamWriter(str);
                //        tw.Write(txtoutput.Text);
                //        txtoutput.Text = "";
                //        tw.Close();

                //    }
                //    if (tbll == null)
                //    {
                //        break;
                //    }

                //}




                //****************************************************************************

                // Generate Controllers

                //****************************************************************************

                foreach (var tbll in tables)
                {
                    if (ChkGenerateControllers.Checked == false)
                    {
                        MessageBox.Show("Scripting Customized Tables For Database " + " " + checkedListBox1.SelectedItem + " " + "completed", "EntityGenerator", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    var pathe = txtscriptoutput.Text;
                    var npathe = path.Replace("\"", "\\");

                    var npa = Path.Combine(npathe, txtprojectname.Text);
                    var Controllers = Path.Combine(npa + "\\" + "Controllers");


                    Directory.CreateDirectory(Controllers);
                    if (cmboutput.Text == "Aspx")
                    {
                        //Repositories = Repositories + "\\" + tbll + "Repository.cs";
                        //GenerateRepositories(tbll);
                    }
                    if (cmboutput.Text == "Mvc")
                    {
                        Controllers = Controllers + "\\" + tbll + "Controller.cs";
                        GenerateControllers(tbll);

                    }

                    txtactivity.AppendText(Environment.NewLine);
                    txtactivity.AppendText(Environment.NewLine);
                    txtactivity.AppendText(Environment.NewLine);

                    using (Stream str = File.Create(Controllers))
                    {

                        TextWriter tw = new StreamWriter(str);
                        tw.Write(txtoutput.Text);
                        txtoutput.Text = "";
                        tw.Close();

                    }

                }
                //*****************************************************************************
                //Generate MVC VIEWs
                //*****************************************************************************

                foreach (var tbll in tables)
                {
                    if (ChkViews.Checked == false)
                    {
                        MessageBox.Show("Scripting Customized Tables For Database " + " " + checkedListBox1.SelectedItem + " " + "completed", "EntityGenerator", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    var pathee = txtscriptoutput.Text;
                    var npathee = path.Replace("\"", "\\");

                    var npea = Path.Combine(npathee, txtprojectname.Text);
                    var views = Path.Combine(npea + "\\" + "Views" + "\\" + tbll);


                    Directory.CreateDirectory(views);
                    if (cmboutput.Text == "Aspx")
                    {

                    }
                    if (cmboutput.Text == "Mvc")
                    {
                        views = views + "\\" + "Index.cshtml";

                        GenerateIndexView(tbll);

                        using (Stream str = File.Create(views))
                        {

                            TextWriter tw = new StreamWriter(str);
                            tw.Write(txtoutput.Text);
                            txtoutput.Text = "";
                            tw.Close();

                        }
                        views = "";
                        views = Path.Combine(npea + "\\" + "Views" + "\\" + tbll) + "\\" + "Create.cshtml";
                        GenerateCreateView(tbll);
                        using (Stream str = File.Create(views))
                        {

                            TextWriter tw = new StreamWriter(str);
                            tw.Write(txtoutput.Text);
                            txtoutput.Text = "";
                            tw.Close();

                        }
                        views = "";
                        views = Path.Combine(npea + "\\" + "Views" + "\\" + tbll) + "\\" + "Edit.cshtml";
                        GenerateEditView(tbll);
                        using (Stream str = File.Create(views))
                        {

                            TextWriter tw = new StreamWriter(str);
                            tw.Write(txtoutput.Text);
                            txtoutput.Text = "";
                            tw.Close();

                        }

                        views = "";
                        views = Path.Combine(npea + "\\" + "Views" + "\\" + tbll) + "\\" + "Details.cshtml";
                        GenerateDetailsView(tbll);
                        using (Stream str = File.Create(views))
                        {

                            TextWriter tw = new StreamWriter(str);
                            tw.Write(txtoutput.Text);
                            txtoutput.Text = "";
                            tw.Close();

                        }

                    }



                    txtactivity.AppendText(Environment.NewLine);

                    txtactivity.AppendText(Environment.NewLine);
                    txtactivity.AppendText(Environment.NewLine);
                    if (tbll == null)
                    {

                        break;
                    }
                }

                //*********************************************************************

                //Generate Customized Repositories

                //*********************************************************************
                foreach (var tbll in tables)
                {

                    var pathe = txtscriptoutput.Text;
                    var npathe = path.Replace("\"", "\\");

                    var npa = Path.Combine(npathe, txtprojectname.Text);
                    var Repositories = Path.Combine(npa + "\\" + txtprojectname.Text + "." + "Repositories" + "\\" + "CustomizedRepositories");


                    Directory.CreateDirectory(Repositories);
                    if (cmboutput.Text == "Aspx")
                    {
                        Repositories = Repositories + "\\" + "Customized" + tbll + "Repository.cs";
                        GenerateCustomizedRepositories(tbll);
                    }
                    if (cmboutput.Text == "Mvc")
                    {
                        Repositories = Repositories + "\\" + "Customized" + tbll + "Repository.cs";
                        GenerateCustomizedRepositories(tbll);

                    }


                    txtactivity.AppendText(Environment.NewLine);

                    txtactivity.AppendText(Environment.NewLine);
                    txtactivity.AppendText(Environment.NewLine);


                    using (Stream str = File.Create(Repositories))
                    {

                        TextWriter tw = new StreamWriter(str);
                        tw.Write(txtoutput.Text);
                        txtoutput.Text = "";
                        tw.Close();

                    }


                }
                MessageBox.Show("Scripting Customized Tables For Database " + " " + checkedListBox1.SelectedItem + " " + "completed", "EntityGenerator", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            catch (Exception ex)
            {

                MessageBox.Show("An Error Occured...If Error Persists..Please Contact the Software Owner", "EntityGenerator", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbDatabaseAuthentication_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDatabaseAuthentication.Text == "True")
            {
                txtConnectionString.Text = "Data Source=.; User Id=.; password=.;";
            }
            else
            {
                txtConnectionString.Text = "Data Source=.; Integrated Security=true;";
            }
        }
        private void LoadDatabases()
        {


            //string[] db = {"master","model","msdb"};
            try
            {


                var con = txtConnectionString.Text;
                var cn = new SqlConnection(con);
                cn.Open();
                var cmd = new SqlCommand("sp_databases", cn) { CommandType = CommandType.StoredProcedure };
                var ds = new DataSet();
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {


                    if (!checkedListBox1.Items.Contains(rdr["DATABASE_NAME"]))
                    {

                        checkedListBox1.Items.Add(rdr["DATABASE_NAME"]);
                    }

                    else
                    {
                        return;
                    }



                }
            }
            catch (Exception Ex)
            {

                //ignored
            }
        }

        private void txtConnectionString_TextChanged(object sender, EventArgs e)
        {
            try
            {


                LoadDatabases();
            }
            catch (Exception)
            {
                MessageBox.Show("No Databases Found", "Entity Gen", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void LoadAllTables()
        {

            if (chkerrlog.Checked == true)
            {
                GenerateErrorLog(checkedListBox1.SelectedItem.ToString());
            }
            var con = txtConnectionString.Text.Trim();
            var cn = new SqlConnection(con + "Initial catalog=" + checkedListBox1.SelectedItem);
            cn.Open();
            string sql = "select name from sysobjects where xtype ='U'";
            var cmd = new SqlCommand();
            cmd = new SqlCommand(sql, cn);

            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {


                tables.Add(rdr["Name"].ToString());

            }

        }

        //Generate DataContracts
        private void WriteToFileMVC(string i)
        {

            var con = txtConnectionString.Text;
            var cn = new SqlConnection(con + "Initial catalog=" + checkedListBox1.SelectedItem);
            cn.Open();
            string sql = "select COLUMN_NAME,DATA_TYPE,IS_NULLABLE from INFORMATION_SCHEMA.COLUMNS IC where TABLE_NAME=@id";

            var cmd = new SqlCommand(sql, cn);

            cmd.Parameters.AddWithValue("@id", i);

            var rdr = cmd.ExecuteReader();

            txtoutput.AppendText("using System;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Collections.Generic;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.ComponentModel;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Data;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Linq;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.ComponentModel.DataAnnotations;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("namespace" + " " + txtprojectname.Text + "." + "DataContracts");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team ");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//*** Template Owner:Okonta Anthony | Email:okontaa@gmail.com | Mobile Number:+2348039436123....");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public partial class " + " " + i.ToString() + "Contract");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");

            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);

            while (rdr.Read())
            {


                if (rdr["DATA_TYPE"].ToString() == "bigint")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText("[Required(ErrorMessage=" + "\" * Required * " + "\")]");
                        txtoutput.AppendText(Environment.NewLine);

                        txtoutput.AppendText("public long " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);

                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public long? " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);


                    }
                }



                //if (rdr["DATA_TYPE"].ToString() == "varchar")
                //{
                //    if (rdr["IS_NULLABLE"].ToString() == "NO")
                //    {

                //        txtoutput.AppendText("[Required(ErrorMessage=" + "\" * Required* " + "\")]");
                //        txtoutput.AppendText(Environment.NewLine);
                //        txtoutput.AppendText("public string " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                //        txtoutput.AppendText(Environment.NewLine);
                //    }
                //    else
                //    {
                //        txtoutput.AppendText(Environment.NewLine);
                //        txtoutput.AppendText("public string " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                //        txtoutput.AppendText(Environment.NewLine);
                //    }
                //}

                if (rdr["DATA_TYPE"].ToString() == "int")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText("[Required(ErrorMessage=" + "\" * Required* " + "\")]");
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public int " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public int ?" + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }

                if (rdr["DATA_TYPE"].ToString() == "binary")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText("[Required(ErrorMessage=" + "\" * Required* " + "\")]");
                        txtoutput.AppendText(Environment.NewLine);

                        txtoutput.AppendText("public Byte[] " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);

                        txtoutput.AppendText("public Byte[] " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "bit")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText("[Required(ErrorMessage=" + "\" * Required* " + "\")]");
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public bool " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public bool ?" + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "date")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText("[Required(ErrorMessage=" + "\" * Required* " + "\")]");
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public DateTime " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public DateTime? " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }

                }
                if (rdr["DATA_TYPE"].ToString() == "datetime")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText("[Required(ErrorMessage=" + "\" * Required* " + "\")]");
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public DateTime " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public DateTime? " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }

                }
                if (rdr["DATA_TYPE"].ToString() == "datetime2")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText("[Required(ErrorMessage=" + "\" * Required* " + "\")]");
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public DateTime " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public DateTime? " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }

                }
                if (rdr["DATA_TYPE"].ToString() == "datetimeoffset")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText("[Required(ErrorMessage=" + "\" * Required* " + "\")]");
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public datetimeoffset " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public datetimeoffset " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }

                }
                if (rdr["DATA_TYPE"].ToString() == "decimal")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText("[Required(ErrorMessage=" + "\" * Required* " + "\")]");
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public decimal " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public decimal ?" + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                //if (rdr["DATA_TYPE"].ToString() == "varbinary")
                //{
                //    if (rdr["IS_NULLABLE"].ToString() == "NO")
                //    {

                //        txtoutput.AppendText("[Required(ErrorMessage=" + "\" * Required* " + "\")]");
                //        txtoutput.AppendText(Environment.NewLine);
                //        txtoutput.AppendText("public byte[] " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                //        txtoutput.AppendText(Environment.NewLine);
                //    }
                //    else
                //    {
                //        txtoutput.AppendText(Environment.NewLine);
                //        txtoutput.AppendText("public byte[] " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                //        txtoutput.AppendText(Environment.NewLine);
                //    }
                //}
                if (rdr["DATA_TYPE"].ToString() == "float")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText("[Required(ErrorMessage=" + "\" * Required* " + "\")]");
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public Double " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);

                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public Double ?" + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "image")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText("[Required(ErrorMessage=" + "\" * Required* " + "\")]");
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public byte[] " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public byte[] " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "money")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText("[Required(ErrorMessage=" + "\" * Required* " + "\")]");
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public Decimal " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public Decimal ?" + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "nchar")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText("[Required(ErrorMessage=" + "\" * Required* " + "\")]");
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public string " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public string " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "ntext")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText("[Required(ErrorMessage=" + "\" * Required* " + "\")]");
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public string " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public string " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "numeric")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText("[Required(ErrorMessage=" + "\" * Required* " + "\")]");
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public Decimal " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public Decimal " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "nvarchar")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText("[Required(ErrorMessage=" + "\" * Required* " + "\")]");
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public string " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public string " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }

                if (rdr["DATA_TYPE"].ToString() == "varchar")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText("[Required(ErrorMessage=" + "\" * Required* " + "\")]");
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public string " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public string " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }


                if (rdr["DATA_TYPE"].ToString() == "real")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText("[Required(ErrorMessage=" + "\" * Required* " + "\")]");
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public string " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public string " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");
                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "rowversion")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText("[Required(ErrorMessage=" + "\" * Required* " + "\")]");
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public single " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public single " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "smalldatetime")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText("[Required(ErrorMessage=" + "\" * Required* " + "\")]");
                        txtoutput.AppendText("public DateTime " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public DateTime? " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "smallint")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText("[Required(ErrorMessage=" + "\" * Required* " + "\")]");
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public int " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public int ?" + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "smallmoney")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText("[Required(ErrorMessage=" + "\" * Required* " + "\")]");
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public Decimal " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public Decimal ?" + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "sql_variant")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText("[Required(ErrorMessage=" + "\" * Required* " + "\")]");
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public Object " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public Object " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "text")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText("[Required(ErrorMessage=" + "\" * Required* " + "\")]");
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public string " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public string " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "time")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText("[Required(ErrorMessage=" + "\" * Required* " + "\")]");
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public timespan " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public timespan " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "timestamp")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText("[Required(ErrorMessage=" + "\" * Required* " + "\")]");
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public Byte[] " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public Byte[] " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "tinyint")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText("[Required(ErrorMessage=" + "\" * Required* " + "\")]");
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public byte " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public byte " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "uniqueidentifier")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText("[Required(ErrorMessage=" + "\" * Required* " + "\")]");
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public Guid " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public Guid " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "varbinary")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText("[Required(ErrorMessage=" + "\" * Required* " + "\")]");
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public Byte[] " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public Byte[] " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "xml")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText("[Required(ErrorMessage=" + "\" * Required* " + "\")]");
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public xml " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);
                    }

                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public xml " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);
                    }
                }

            }
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team //***");

        }

        private void WriteToFileASP(string i)
        {
            var con = txtConnectionString.Text;
            var cn = new SqlConnection(con + "Initial catalog=" + checkedListBox1.SelectedItem);
            cn.Open();
            string sql = "select COLUMN_NAME,DATA_TYPE,IS_NULLABLE from INFORMATION_SCHEMA.COLUMNS IC where TABLE_NAME=@id";

            var cmd = new SqlCommand(sql, cn);

            cmd.Parameters.AddWithValue("@id", i);

            var rdr = cmd.ExecuteReader();

            txtoutput.AppendText("using System;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Collections.Generic;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.ComponentModel;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Data;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Linq;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("namespace" + " " + txtprojectname.Text + "." + "DataContracts");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team ");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//*** Template Owner:Okonta Anthony | Email:okontaa@gmail.com | Mobile Number:+2348039436123....");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public partial class " + " " + i.ToString() + "Contract");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");

            txtoutput.AppendText(Environment.NewLine);
            while (rdr.Read())
            {


                if (rdr["DATA_TYPE"].ToString() == "bigint")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);

                        txtoutput.AppendText("public long " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);

                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public long? " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);


                    }
                }

                if (rdr["DATA_TYPE"].ToString() == "int")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public int " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public int ?" + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }

                if (rdr["DATA_TYPE"].ToString() == "binary")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);

                        txtoutput.AppendText("public Byte[] " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);

                        txtoutput.AppendText("public Byte[] " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "bit")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public bool " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public bool ?" + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "date")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public DateTime " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public DateTime? " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }

                }
                if (rdr["DATA_TYPE"].ToString() == "datetime")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public DateTime " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public DateTime? " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }

                }
                if (rdr["DATA_TYPE"].ToString() == "datetime2")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public DateTime " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public DateTime? " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }

                }
                if (rdr["DATA_TYPE"].ToString() == "datetimeoffset")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public datetimeoffset " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public datetimeoffset " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }

                }
                if (rdr["DATA_TYPE"].ToString() == "decimal")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public decimal " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public decimal ?" + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "varbinary")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public byte[] " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public byte[] " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "float")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public Double " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);

                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public Double ?" + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "image")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public byte[] " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public byte[] " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "money")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public Decimal " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public Decimal ?" + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "nchar")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public string " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public string " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "ntext")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public string " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public string " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "numeric")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public Decimal " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public Decimal " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "nvarchar")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public string " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public string " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "real")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public string " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public string " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");
                        //  txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "rowversion")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public single " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public single " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "smalldatetime")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public DateTime " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public DateTime? " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "smallint")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public int " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public int ?" + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "smallmoney")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public Decimal " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public Decimal ?" + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "sql_variant")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public Object " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public Object " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "text")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public string " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public string " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "time")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public timespan " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public timespan " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "timestamp")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public Byte[] " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public Byte[] " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "tinyint")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public byte " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public byte " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "uniqueidentifier")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public Guid " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public Guid " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "varbinary")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public Byte[] " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public Byte[] " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");

                        txtoutput.AppendText(Environment.NewLine);
                    }
                }
                if (rdr["DATA_TYPE"].ToString() == "xml")
                {
                    if (rdr["IS_NULLABLE"].ToString() == "NO")
                    {

                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public xml " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);
                    }

                    else
                    {
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText("public xml " + rdr["COLUMN_NAME"].ToString() + "{get;set;}");
                        txtoutput.AppendText(Environment.NewLine);
                        txtoutput.AppendText(Environment.NewLine);
                    }
                }

            }
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team //***");

        }

        private void WriteToFileAndroid(string i)
        {
            var con = txtConnectionString.Text;
            var cn = new SqlConnection(con + "Initial catalog=" + checkedListBox1.SelectedItem);
            cn.Open();
            string sql = "select COLUMN_NAME,DATA_TYPE,IS_NULLABLE from INFORMATION_SCHEMA.COLUMNS IC where TABLE_NAME=@id";

            var cmd = new SqlCommand(sql, cn);

            cmd.Parameters.AddWithValue("@id", i);

            var rdr = cmd.ExecuteReader();


            txtoutput.AppendText("//These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team. ");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("package" + " " + txtprojectname.Text.Trim() + ";");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("/**");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("* @author EntityGenerator");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("*");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("*/");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public class " + " " + i.ToString());
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            while (rdr.Read())
            {


                if (rdr["DATA_TYPE"].ToString() == "bigint")
                {


                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("private long " + " " + rdr["COLUMN_NAME"].ToString() + "; ");
                    txtoutput.AppendText(Environment.NewLine);



                }

                if (rdr["DATA_TYPE"].ToString() == "int")
                {


                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("private int" + " " + rdr["COLUMN_NAME"].ToString() + " ;");

                    txtoutput.AppendText(Environment.NewLine);

                }

                if (rdr["DATA_TYPE"].ToString() == "binary")
                {


                    txtoutput.AppendText(Environment.NewLine);

                    txtoutput.AppendText("private Byte[] " + " " + rdr["COLUMN_NAME"].ToString() + ";");

                    txtoutput.AppendText(Environment.NewLine);

                }
                if (rdr["DATA_TYPE"].ToString() == "bit")
                {


                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("private boolean " + " " + rdr["COLUMN_NAME"].ToString() + ";");

                    txtoutput.AppendText(Environment.NewLine);

                }
                if (rdr["DATA_TYPE"].ToString() == "date")
                {


                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("private String " + " " + rdr["COLUMN_NAME"].ToString() + ";");

                    txtoutput.AppendText(Environment.NewLine);


                }
                if (rdr["DATA_TYPE"].ToString() == "datetime")
                {


                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("private String " + " " + rdr["COLUMN_NAME"].ToString() + ";");

                    txtoutput.AppendText(Environment.NewLine);


                }
                if (rdr["DATA_TYPE"].ToString() == "datetime2")
                {


                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("private String " + " " + rdr["COLUMN_NAME"].ToString() + ";");

                    txtoutput.AppendText(Environment.NewLine);


                }
                if (rdr["DATA_TYPE"].ToString() == "datetimeoffset")
                {


                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("public String " + " " + rdr["COLUMN_NAME"].ToString() + ";");

                    txtoutput.AppendText(Environment.NewLine);


                }
                if (rdr["DATA_TYPE"].ToString() == "decimal")
                {

                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("private Double " + " " + rdr["COLUMN_NAME"].ToString() + ";");

                    txtoutput.AppendText(Environment.NewLine);

                }
                if (rdr["DATA_TYPE"].ToString() == "varbinary")
                {


                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("private byte[] " + " " + rdr["COLUMN_NAME"].ToString() + ";");

                    txtoutput.AppendText(Environment.NewLine);

                }
                if (rdr["DATA_TYPE"].ToString() == "float")
                {


                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("private Double " + " " + rdr["COLUMN_NAME"].ToString() + ";");

                    txtoutput.AppendText(Environment.NewLine);

                }
                if (rdr["DATA_TYPE"].ToString() == "image")
                {


                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("private byte[] " + " " + rdr["COLUMN_NAME"].ToString() + ";");

                    txtoutput.AppendText(Environment.NewLine);

                }
                if (rdr["DATA_TYPE"].ToString() == "money")
                {


                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("private Decimal " + " " + rdr["COLUMN_NAME"].ToString() + ";");

                    txtoutput.AppendText(Environment.NewLine);

                }
                if (rdr["DATA_TYPE"].ToString() == "nchar")
                {


                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("private String " + " " + rdr["COLUMN_NAME"].ToString() + ";");

                    txtoutput.AppendText(Environment.NewLine);

                }
                if (rdr["DATA_TYPE"].ToString() == "ntext")
                {


                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("private String " + " " + rdr["COLUMN_NAME"].ToString() + ";");

                    txtoutput.AppendText(Environment.NewLine);

                }
                if (rdr["DATA_TYPE"].ToString() == "numeric")
                {


                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("private Decimal " + " " + rdr["COLUMN_NAME"].ToString() + ";");

                    txtoutput.AppendText(Environment.NewLine);

                }
                if (rdr["DATA_TYPE"].ToString() == "nvarchar")
                {


                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("public String " + " " + rdr["COLUMN_NAME"].ToString() + ";");

                    txtoutput.AppendText(Environment.NewLine);

                }
                if (rdr["DATA_TYPE"].ToString() == "real")
                {


                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("public String " + " " + rdr["COLUMN_NAME"].ToString() + ";");
                    //  txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(Environment.NewLine);

                }

                if (rdr["DATA_TYPE"].ToString() == "smalldatetime")
                {


                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("private String " + " " + rdr["COLUMN_NAME"].ToString() + ";");

                    txtoutput.AppendText(Environment.NewLine);

                }
                if (rdr["DATA_TYPE"].ToString() == "smallint")
                {


                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("private int " + " " + rdr["COLUMN_NAME"].ToString() + ";");

                    txtoutput.AppendText(Environment.NewLine);

                }
                if (rdr["DATA_TYPE"].ToString() == "smallmoney")
                {


                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("private Decimal " + " " + rdr["COLUMN_NAME"].ToString() + ";");

                    txtoutput.AppendText(Environment.NewLine);

                }

                if (rdr["DATA_TYPE"].ToString() == "text")
                {


                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("private string " + " " + rdr["COLUMN_NAME"].ToString() + ";");

                    txtoutput.AppendText(Environment.NewLine);

                }

                if (rdr["DATA_TYPE"].ToString() == "timestamp")
                {


                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("private Byte[] " + " " + rdr["COLUMN_NAME"].ToString() + ";");

                    txtoutput.AppendText(Environment.NewLine);

                }


                if (rdr["DATA_TYPE"].ToString() == "varbinary")
                {


                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("private Byte[] " + " " + rdr["COLUMN_NAME"].ToString() + ";");

                    txtoutput.AppendText(Environment.NewLine);

                }


            }
            txtoutput.AppendText(Environment.NewLine);
        }
        private void GenerateAndroid2(string i)
        {

            var con = txtConnectionString.Text;
            var cn = new SqlConnection(con + "Initial catalog=" + checkedListBox1.SelectedItem);
            cn.Open();
            string sql = "select COLUMN_NAME,DATA_TYPE,IS_NULLABLE from INFORMATION_SCHEMA.COLUMNS IC where TABLE_NAME=@id";

            var cmd = new SqlCommand(sql, cn);

            cmd.Parameters.AddWithValue("@id", i);

            var rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {

                if (rdr["DATA_TYPE"].ToString() == "bigint")
                {
                    txtoutput.AppendText("public void " + "set" + rdr["COLUMN_NAME"].ToString() + "(long " + rdr["COLUMN_NAME"].ToString().ToLower() + ") ");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("{");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(rdr["COLUMN_NAME"].ToString() + "=" + rdr["COLUMN_NAME"].ToString().ToLower() + ";");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("}");
                    txtoutput.AppendText(Environment.NewLine);
                }

                if (rdr["DATA_TYPE"].ToString() == "int")
                {
                    txtoutput.AppendText("public void " + "set" + rdr["COLUMN_NAME"].ToString() + "(int " + rdr["COLUMN_NAME"].ToString().ToLower() + ") ");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("{");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(rdr["COLUMN_NAME"].ToString() + "=" + rdr["COLUMN_NAME"].ToString().ToLower() + ";");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("}");
                    txtoutput.AppendText(Environment.NewLine);

                }

                if (rdr["DATA_TYPE"].ToString() == "binary")
                {

                    txtoutput.AppendText("public void " + "set" + rdr["COLUMN_NAME"].ToString() + "(Byte[] " + rdr["COLUMN_NAME"].ToString().ToLower() + ") ");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("{");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(rdr["COLUMN_NAME"].ToString() + "=" + rdr["COLUMN_NAME"].ToString().ToLower() + ";");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("}");
                    txtoutput.AppendText(Environment.NewLine);


                }
                if (rdr["DATA_TYPE"].ToString() == "bit")
                {




                    txtoutput.AppendText("public void " + "set" + rdr["COLUMN_NAME"].ToString() + "(boolean " + rdr["COLUMN_NAME"].ToString().ToLower() + ") ");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("{");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(rdr["COLUMN_NAME"].ToString() + "=" + rdr["COLUMN_NAME"].ToString().ToLower() + ";");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("}");
                    txtoutput.AppendText(Environment.NewLine);

                }
                if (rdr["DATA_TYPE"].ToString() == "date")
                {

                    txtoutput.AppendText("public void " + "set" + rdr["COLUMN_NAME"].ToString() + "(String " + rdr["COLUMN_NAME"].ToString().ToLower() + ") ");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("{");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(rdr["COLUMN_NAME"].ToString() + "=" + rdr["COLUMN_NAME"].ToString().ToLower() + ";");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("}");
                    txtoutput.AppendText(Environment.NewLine);

                }
                if (rdr["DATA_TYPE"].ToString() == "datetime")
                {

                    txtoutput.AppendText("public void " + "set" + rdr["COLUMN_NAME"].ToString() + "(String " + rdr["COLUMN_NAME"].ToString().ToLower() + ") ");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("{");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(rdr["COLUMN_NAME"].ToString() + "=" + rdr["COLUMN_NAME"].ToString().ToLower() + ";");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("}");
                    txtoutput.AppendText(Environment.NewLine);


                }
                if (rdr["DATA_TYPE"].ToString() == "datetime2")
                {

                    txtoutput.AppendText("public void " + "set" + rdr["COLUMN_NAME"].ToString() + "(String " + rdr["COLUMN_NAME"].ToString().ToLower() + ") ");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("{");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(rdr["COLUMN_NAME"].ToString() + "=" + rdr["COLUMN_NAME"].ToString().ToLower() + ";");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("}");
                    txtoutput.AppendText(Environment.NewLine);

                }
                if (rdr["DATA_TYPE"].ToString() == "datetimeoffset")
                {

                    txtoutput.AppendText("public void " + "set" + rdr["COLUMN_NAME"].ToString() + "(String " + rdr["COLUMN_NAME"].ToString().ToLower() + ") ");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("{");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(rdr["COLUMN_NAME"].ToString() + "=" + rdr["COLUMN_NAME"].ToString().ToLower() + ";");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("}");
                    txtoutput.AppendText(Environment.NewLine);
                }
                if (rdr["DATA_TYPE"].ToString() == "decimal")
                {

                    txtoutput.AppendText("public void " + "set" + rdr["COLUMN_NAME"].ToString() + "(Double " + rdr["COLUMN_NAME"].ToString().ToLower() + ") ");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("{");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(rdr["COLUMN_NAME"].ToString() + "=" + rdr["COLUMN_NAME"].ToString().ToLower() + ";");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("}");
                    txtoutput.AppendText(Environment.NewLine);
                }
                if (rdr["DATA_TYPE"].ToString() == "varbinary")
                {

                    txtoutput.AppendText("public void " + "set" + rdr["COLUMN_NAME"].ToString() + "(byte[] " + rdr["COLUMN_NAME"].ToString().ToLower() + ") ");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("{");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(rdr["COLUMN_NAME"].ToString() + "=" + rdr["COLUMN_NAME"].ToString().ToLower() + ";");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("}");
                    txtoutput.AppendText(Environment.NewLine);
                }
                if (rdr["DATA_TYPE"].ToString() == "float")
                {

                    txtoutput.AppendText("public void " + "set" + rdr["COLUMN_NAME"].ToString() + "(Double " + rdr["COLUMN_NAME"].ToString().ToLower() + ") ");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("{");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(rdr["COLUMN_NAME"].ToString() + "=" + rdr["COLUMN_NAME"].ToString().ToLower() + ";");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("}");
                    txtoutput.AppendText(Environment.NewLine);
                }
                if (rdr["DATA_TYPE"].ToString() == "image")
                {



                    txtoutput.AppendText("public void " + "set" + rdr["COLUMN_NAME"].ToString() + "(byte[] " + rdr["COLUMN_NAME"].ToString().ToLower() + ") ");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("{");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(rdr["COLUMN_NAME"].ToString() + "=" + rdr["COLUMN_NAME"].ToString().ToLower() + ";");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("}");
                    txtoutput.AppendText(Environment.NewLine);
                }
                if (rdr["DATA_TYPE"].ToString() == "money")
                {



                    txtoutput.AppendText("public void " + "set" + rdr["COLUMN_NAME"].ToString() + "(Decimal " + rdr["COLUMN_NAME"].ToString().ToLower() + ") ");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("{");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(rdr["COLUMN_NAME"].ToString() + "=" + rdr["COLUMN_NAME"].ToString().ToLower() + ";");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("}");
                    txtoutput.AppendText(Environment.NewLine);
                }
                if (rdr["DATA_TYPE"].ToString() == "nchar")
                {



                    txtoutput.AppendText("public void " + "set" + rdr["COLUMN_NAME"].ToString() + "(String " + rdr["COLUMN_NAME"].ToString().ToLower() + ") ");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("{");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(rdr["COLUMN_NAME"].ToString() + "=" + rdr["COLUMN_NAME"].ToString().ToLower() + ";");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("}");
                    txtoutput.AppendText(Environment.NewLine);
                }
                if (rdr["DATA_TYPE"].ToString() == "ntext")
                {

                    txtoutput.AppendText("public void " + "set" + rdr["COLUMN_NAME"].ToString() + "(String " + rdr["COLUMN_NAME"].ToString().ToLower() + ") ");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("{");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(rdr["COLUMN_NAME"].ToString() + "=" + rdr["COLUMN_NAME"].ToString().ToLower() + ";");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("}");
                    txtoutput.AppendText(Environment.NewLine);
                }
                if (rdr["DATA_TYPE"].ToString() == "numeric")
                {

                    txtoutput.AppendText("public void " + "set" + rdr["COLUMN_NAME"].ToString() + "(Decimal " + rdr["COLUMN_NAME"].ToString().ToLower() + ") ");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("{");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(rdr["COLUMN_NAME"].ToString() + "=" + rdr["COLUMN_NAME"].ToString().ToLower() + ";");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("}");
                    txtoutput.AppendText(Environment.NewLine);
                }
                if (rdr["DATA_TYPE"].ToString() == "nvarchar")
                {

                    txtoutput.AppendText("public void " + "set" + rdr["COLUMN_NAME"].ToString() + "(String " + rdr["COLUMN_NAME"].ToString().ToLower() + ") ");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("{");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(rdr["COLUMN_NAME"].ToString() + "=" + rdr["COLUMN_NAME"].ToString().ToLower() + ";");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("}");
                    txtoutput.AppendText(Environment.NewLine);
                }
                if (rdr["DATA_TYPE"].ToString() == "real")
                {

                    txtoutput.AppendText("public void " + "set" + rdr["COLUMN_NAME"].ToString() + "(String " + rdr["COLUMN_NAME"].ToString().ToLower() + ") ");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("{");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(rdr["COLUMN_NAME"].ToString() + "=" + rdr["COLUMN_NAME"].ToString().ToLower() + ";");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("}");
                    txtoutput.AppendText(Environment.NewLine);
                }

                if (rdr["DATA_TYPE"].ToString() == "smalldatetime")
                {

                    txtoutput.AppendText("public void " + "set" + rdr["COLUMN_NAME"].ToString() + "(String " + rdr["COLUMN_NAME"].ToString().ToLower() + ") ");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("{");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(rdr["COLUMN_NAME"].ToString() + "=" + rdr["COLUMN_NAME"].ToString().ToLower() + ";");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("}");
                    txtoutput.AppendText(Environment.NewLine);
                }
                if (rdr["DATA_TYPE"].ToString() == "smallint")
                {

                    txtoutput.AppendText("public void " + "set" + rdr["COLUMN_NAME"].ToString() + "(int " + rdr["COLUMN_NAME"].ToString().ToLower() + ") ");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("{");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(rdr["COLUMN_NAME"].ToString() + "=" + rdr["COLUMN_NAME"].ToString().ToLower() + ";");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("}");
                    txtoutput.AppendText(Environment.NewLine);
                }
                if (rdr["DATA_TYPE"].ToString() == "smallmoney")
                {

                    txtoutput.AppendText("public void " + "set" + rdr["COLUMN_NAME"].ToString() + "(Decimal " + rdr["COLUMN_NAME"].ToString().ToLower() + ") ");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("{");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(rdr["COLUMN_NAME"].ToString() + "=" + rdr["COLUMN_NAME"].ToString().ToLower() + ";");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("}");
                    txtoutput.AppendText(Environment.NewLine);
                }

                if (rdr["DATA_TYPE"].ToString() == "text")
                {




                    txtoutput.AppendText("public void " + "set" + rdr["COLUMN_NAME"].ToString() + "(String " + rdr["COLUMN_NAME"].ToString().ToLower() + ") ");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("{");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(rdr["COLUMN_NAME"].ToString() + "=" + rdr["COLUMN_NAME"].ToString().ToLower() + ";");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("}");
                    txtoutput.AppendText(Environment.NewLine);
                }

                if (rdr["DATA_TYPE"].ToString() == "timestamp")
                {


                    txtoutput.AppendText("public void " + "set" + rdr["COLUMN_NAME"].ToString() + "(Byte[] " + rdr["COLUMN_NAME"].ToString().ToLower() + ") ");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("{");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(rdr["COLUMN_NAME"].ToString() + "=" + rdr["COLUMN_NAME"].ToString().ToLower() + ";");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("}");
                }


                if (rdr["DATA_TYPE"].ToString() == "varbinary")
                {
                    txtoutput.AppendText("public void " + "set" + rdr["COLUMN_NAME"].ToString() + "(Byte[] " + rdr["COLUMN_NAME"].ToString().ToLower() + ") ");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("{");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(rdr["COLUMN_NAME"].ToString() + "=" + rdr["COLUMN_NAME"].ToString().ToLower() + ";");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("}");
                    txtoutput.AppendText(Environment.NewLine);

                }
                txtoutput.AppendText(Environment.NewLine);

            }
            txtoutput.AppendText("}");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtoutput.Visible = false;
            label7.Text = "CopyRight" + " " + DateTime.Now.Year.ToString();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var about = new About();
            about.Show();
            this.Hide();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var help = new Help();
            help.Show();
            this.Hide();
        }

        private void CustomizedDataContracts(string i)
        {
            txtoutput.AppendText("using System;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Collections.Generic;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.ComponentModel;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Data;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Linq;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.ComponentModel.DataAnnotations;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("namespace" + " " + txtprojectname.Text + "." + "DataContracts");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team ");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//*** Template Owner:Okonta Anthony | Email:okontaa@gmail.com | Mobile Number:+2348039436123....");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public partial class " + " " + i.ToString() + "Contract");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team //***");
        }

        private void GenerateServiceContracts(string i)
        {
            txtoutput.AppendText("using System;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Collections.Generic;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.ComponentModel;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Data;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Linq;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using " + txtprojectname.Text + "." + "DataContracts;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using " + txtprojectname.Text + "." + "Repositories;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using " + txtprojectname.Text + "." + "Services;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Linq;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Linq.Expressions;");
            //txtoutput.AppendText("using System.ComponentModel.DataAnnotations;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("namespace" + " " + txtprojectname.Text + "." + "Services");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team ");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//*** Template Owner:Okonta Anthony | Email:okontaa@gmail.com | Mobile Number:+2348039436123....");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public partial class " + " " + i + "Services");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("private  readonly " + i + "Repository" + " " + "_repository;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public" + " " + i + "Services()");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("_repository =" + " new " + i + "Repository" + "();");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            //txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            //********************************************************GetAll()
            txtoutput.AppendText("public IEnumerable<" + i + "Contract" + "> GetAll()");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return _repository.GetAll" + i + "();");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);



            // ********************************************************GetbyId()
            txtoutput.AppendText("public " + " " + i + "Contract" + " " + "Get" + i + "(int id)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return _repository.Get" + i + "(id);");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);


            // *********************************************************Insert(T model)


            txtoutput.AppendText("public int Add" + i + "(" + i + "Contract" + " " + "model)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return _repository ." + "Add" + i + "(model);");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);

            //*********************************************************Update(T model)

            txtoutput.AppendText("public bool Update(" + i + "Contract" + " " + "model" + ")");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return _repository.Update(model);");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");



            //*********************************************************GetAllByExpression(int id)



            txtoutput.AppendText("public IQueryable<" + i + "Contract" + "> GetByExpression(Expression<Func<T, bool>> predicate)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return _repository.GetAllByExpression(predicate);");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");

            txtoutput.AppendText(Environment.NewLine);


            ////*********************************************************GetSingleByExpression(int id)



            txtoutput.AppendText("public IQueryable<" + i + "Contract" + "> GetSingleByExpression(Expression<Func<T, bool>> predicate)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return _repository.GetAllByExpression(predicate);");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            //*********************************************************Delete(int id)

            txtoutput.AppendText("public int Delete(int id)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return _repository.Delete(id);");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team //***");
        }

        private void GenerateCustomizedServiceContracts(string i)
        {

            txtoutput.AppendText("using System;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Collections.Generic;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.ComponentModel;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Data;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Linq;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using " + txtprojectname.Text + "." + "DataContracts;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using " + txtprojectname.Text + "." + "Repositories;");

            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("namespace" + " " + txtprojectname.Text + "." + "Services");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team ");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//*** Template Owner:Okonta Anthony | Email:okontaa@gmail.com | Mobile Number:+2348039436123....");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public partial class " + " " + i + "Services");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            //  txtoutput.AppendText(txtEntitiesName.Text.Trim() + " " + "_dbcontextcustomized=new" + " " + txtEntitiesName.Text.Trim() + "();");
            txtoutput.AppendText(Environment.NewLine);
            //txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team //***");
        }

        private void GenerateServiceContractInterface()
        {

            txtoutput.AppendText("using System;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Collections.Generic;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.ComponentModel;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Data;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Linq;");
            txtoutput.AppendText(Environment.NewLine);
            //txtoutput.AppendText("using System.ComponentModel.DataAnnotations;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("namespace" + " " + txtprojectname.Text + "." + "Services");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team ");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//*** Template Owner:Okonta Anthony | Email:okontaa@gmail.com | Mobile Number:+2348039436123....");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public interface Iserviceprovider");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");

            foreach (var tbl in tables)
            {
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText(tbl + "Services" + " " + "Get" + tbl + "Services" + "();");
                txtoutput.AppendText(Environment.NewLine);
            }

            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team //***");
        }

        private void GenerateServiceContractStandardServiceProvider()
        {


            txtoutput.AppendText("using System;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Collections.Generic;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.ComponentModel;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Data;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Linq;");
            txtoutput.AppendText(Environment.NewLine);
            //txtoutput.AppendText("using System.ComponentModel.DataAnnotations;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("namespace" + " " + txtprojectname.Text + "." + "Services");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team ");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//*** Template Owner:Okonta Anthony | Email:okontaa@gmail.com | Mobile Number:+2348039436123....");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public partial class StandardServiceProvider:Iserviceprovider");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");

            foreach (var tbl in tables)
            {
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText("public" + " " + tbl + "Services" + " " + "Get" + tbl + "Services()");
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText("{");
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText("return new " + " " + tbl + "Services();");
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText("}");
            }

            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team //***");
        }

        private void GenerateServiceContractServiceProvider()
        {
            txtoutput.AppendText("using System;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Collections.Generic;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.ComponentModel;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Data;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Linq;");
            txtoutput.AppendText(Environment.NewLine);
            //txtoutput.AppendText("using System.ComponentModel.DataAnnotations;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("namespace" + " " + txtprojectname.Text + "." + "Services");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team ");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//*** Template Owner:Okonta Anthony | Email:okontaa@gmail.com | Mobile Number:+2348039436123....");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public class ServiceProvider");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("private Iserviceprovider _iServices = null;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("private static ServiceProvider _newInstance;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public ServiceProvider()");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(" _iServices = new StandardServiceProvider();");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public static Iserviceprovider Instance()");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(" if(_newInstance == null)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("_newInstance = new ServiceProvider();");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return _newInstance._iServices;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team //***");
        }


        //Generate Infrastructure

        private void GenerateInfrastructure()
        {

            txtoutput.AppendText("using System;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Collections.Generic;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.ComponentModel;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Data;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Linq;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Linq.Expressions;");

            txtoutput.AppendText(Environment.NewLine);
            //txtoutput.AppendText("using System.ComponentModel.DataAnnotations;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("namespace" + " " + txtprojectname.Text + "." + "Infrastructure");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team ");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//*** Template Owner:Okonta Anthony | Email:okontaa@gmail.com | Mobile Number:+2348039436123....");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            //txtoutput.AppendText("public interface IRepository<T> where T:class");
            txtoutput.AppendText("public interface IRepository<T> where T:class");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("void Add(T model);");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("void Remove(T model);");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("void Update(T model);");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("IEnumerable<T> GetAll();");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);

            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("T GetAllById(int id);");
            txtoutput.AppendText(Environment.NewLine);

            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("IQueryable<T> FindByExpressions(Expression<Func<T, bool>> predicate);");
            txtoutput.AppendText(Environment.NewLine);


            txtoutput.AppendText("}");

            txtoutput.AppendText(Environment.NewLine);


            txtoutput.AppendText(Environment.NewLine);
            //txtoutput.AppendText("public interface IUnitow:IDisposable");
            txtoutput.AppendText("public interface IUnitow<T>:IDisposable where T:class");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("void Save();");

            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team //***");
        }

        public List<check> gettableloop(string i)
        {
            var con = txtConnectionString.Text;
            var cn = new SqlConnection(con + "Initial catalog=" + checkedListBox1.SelectedItem);
            cn.Open();
            string sql = "select COLUMN_NAME,DATA_TYPE,IS_NULLABLE from INFORMATION_SCHEMA.COLUMNS IC where TABLE_NAME=@id";

            var cmd = new SqlCommand(sql, cn);

            cmd.Parameters.AddWithValue("@id", i);

            var rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                var check = new check
                {
                    isnull = rdr["IS_NULLABLE"].ToString(),
                    datatype = rdr["DATA_TYPE"].ToString(),
                    columnname = rdr["COLUMN_NAME"].ToString()
                };

                list.Add(check);

            }

            return list.ToList();
        }

        //private void GenerateRepositories(string i)
        //{
        //    txtoutput.AppendText("using System;");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("using System.Collections.Generic;");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("using System.ComponentModel;");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("using System.Data.Entity;");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("using System.Data;");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("using System.Linq;");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("using" + " " + txtprojectname.Text + "." + "Infrastructure;");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("using" + " " + txtprojectname.Text + "." + "Ef;");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("using" + " " + txtprojectname.Text + "." + "DataContracts;");
        //    txtoutput.AppendText(Environment.NewLine);
        //    //txtoutput.AppendText("using System.ComponentModel.DataAnnotations;");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("namespace" + " " + txtprojectname.Text + "." + "Repositories");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("//These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team ");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("//*** Template Owner:Okonta Anthony | Email:okontaa@gmail.com | Mobile Number:+2348039436123....");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("{");
        //    txtoutput.AppendText(Environment.NewLine);
        // //   txtoutput.AppendText("public partial class " + " " + i + "Repository" + ":IRepository<" + i + "Contract" + ">");
        //    txtoutput.AppendText("public partial class " + " " + i + "Repository" + ":IRepository<" + i + "Contract" + ">");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("{");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("private Uow _repository;");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("public " + " " + i + "Repository()");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("{");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("_repository = new Uow();      //*Make an instance to your unit of work class");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("}");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText(Environment.NewLine);
        //    //txtoutput.AppendText(txtEntitiesName.Text.Trim() + " " + "_dbcontext=new" + " " + txtEntitiesName.Text.Trim() + "();");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("public IEnumerable<" + i + "Contract" + ">findAll()");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("{");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("try");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("{");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText(Environment.NewLine);
        //    if (i.EndsWith("y"))
        //    {
        //        var newi = i.Replace("y", "ies");
        //        txtoutput.AppendText("var all =(from _" + i + " " + "in _repository._entity." + newi);
        //    }
        //    else if (i.EndsWith("s"))
        //    {
        //        txtoutput.AppendText("var all =(from _" + i + " " + "in _repository._entity." + i);
        //    }
        //    else if (i.EndsWith("ch"))
        //    {
        //        txtoutput.AppendText("var all =(from _" + i + " " + "in _repository._entity." + i + "es");
        //    }
        //    else if (i.EndsWith("sh"))
        //    {
        //        txtoutput.AppendText("var all =(from _" + i + " " + "in _repository._entity." + i + "es");
        //    }
        //    else if (i.EndsWith("s"))
        //    {
        //        txtoutput.AppendText("var all =(from _" + i + " " + "in _repository._entity." + i);
        //    }
        //    else
        //    {
        //        txtoutput.AppendText("var all =(from _" + i + " " + "in _repository._entity." + i + "s");
        //    }
        //    txtoutput.AppendText(Environment.NewLine);

        //    txtoutput.AppendText(Environment.NewLine);

        //    txtoutput.AppendText("select new " + i + "Contract");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("{");
        //    gettableloop(i);
        //    foreach (var li in list)
        //    {
        //        txtoutput.AppendText(Environment.NewLine);



        //        if (li.datatype == "int" && li.isnull == "YES")
        //        {

        //            txtoutput.AppendText(li.columnname + "=" + "_" + i + "." + li.columnname + ".Value,");
        //        }
        //        else if (li.datatype == "bit" && li.isnull == "YES")
        //        {

        //            txtoutput.AppendText(li.columnname + "=" + "_" + i + "." + li.columnname + ".Value,");
        //        }
        //        else if (li.datatype == "datetime" && li.isnull == "YES")
        //        {

        //            txtoutput.AppendText(li.columnname + "=" + "_" + i + "." + li.columnname + ".Value,");
        //        }
        //        else if (li.datatype == "long" && li.isnull == "YES")
        //        {

        //            txtoutput.AppendText(li.columnname + "=" + "_" + i + "." + li.columnname + ".Value,");
        //        }


        //        else
        //        {
        //            txtoutput.AppendText(li.columnname + "=" + "_" + i + "." + li.columnname + ",");
        //        }

        //        txtoutput.AppendText(Environment.NewLine);
        //        txtoutput.AppendText(Environment.NewLine);
        //    }

        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("}).ToList();");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("return all;");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("}");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("catch(Exception ex)");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("{");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("//*** call your write to error log method or do what ever u like with the exception");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("return new List<" + i + "Contract" + ">();");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("}");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("}");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText(Environment.NewLine);

        //    txtoutput.AppendText("public IEnumerable<" + i + "Contract" + ">findAllByid(int id)");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("{");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("try");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("{");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText(Environment.NewLine);
        //    if (i.EndsWith("y"))
        //    {
        //        var newi = i.Replace("y", "ies");
        //        txtoutput.AppendText("var all =(from _" + i + " " + "in _repository._entity." + i + " " + " where  _" + i + "." + i + "Id==id");
        //    }
        //    else if (i.EndsWith("s"))
        //    {
        //        txtoutput.AppendText("var all =(from _" + i + " " + "in _repository._entity." + i + " " + " where  _" + i + "." + i + "Id==id");
        //    }
        //    else if (i.EndsWith("ch"))
        //    {
        //        txtoutput.AppendText("var all =(from _" + i + " " + "in _repository._entity." + i + "es" + " " + " where  _" + i + "." + i + "Id==id");
        //    }
        //    else if (i.EndsWith("sh"))
        //    {
        //        txtoutput.AppendText("var all =(from _" + i + " " + "in _repository._entity." + i + "es" + " " + " where  _" + i + "." + i + "Id==id");
        //    }
        //    else if (i.EndsWith("s"))
        //    {
        //        txtoutput.AppendText("var all =(from _" + i + " " + "in _repository._entity." + i + " " + " where  _" + i + "." + i + "Id==id");
        //    }
        //    else
        //    {
        //        txtoutput.AppendText("var all =(from _" + i + " " + "in _repository._entity." + i + "s" + " " + " where  _" + i + "." + i + "Id==id");
        //    }
        //    txtoutput.AppendText(Environment.NewLine);

        //    txtoutput.AppendText(Environment.NewLine);

        //    txtoutput.AppendText("select new " + i + "Contract");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("{");
        //    // gettableloop(i);
        //    foreach (var li in list)
        //    {
        //        txtoutput.AppendText(Environment.NewLine);



        //        if (li.datatype == "int" && li.isnull == "YES")
        //        {

        //            txtoutput.AppendText(li.columnname + "=" + "_" + i + "." + li.columnname + ".Value,");
        //        }
        //        else if (li.datatype == "bit" && li.isnull == "YES")
        //        {

        //            txtoutput.AppendText(li.columnname + "=" + "_" + i + "." + li.columnname + ".Value,");
        //        }
        //        else if (li.datatype == "datetime" && li.isnull == "YES")
        //        {

        //            txtoutput.AppendText(li.columnname + "=" + "_" + i + "." + li.columnname + ".Value,");
        //        }
        //        else if (li.datatype == "long" && li.isnull == "YES")
        //        {

        //            txtoutput.AppendText(li.columnname + "=" + "_" + i + "." + li.columnname + ".Value,");
        //        }


        //        else
        //        {
        //            txtoutput.AppendText(li.columnname + "=" + "_" + i + "." + li.columnname + ",");
        //        }

        //        txtoutput.AppendText(Environment.NewLine);
        //        txtoutput.AppendText(Environment.NewLine);
        //    }

        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("}).ToList();");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("return all;");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("}");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("catch(Exception ex)");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("{");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("//*** call your write to error log method or do what ever u like with the exception");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("return new List<" + i + "Contract" + ">();");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("}");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("}");


        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("public " + i + "Contract" + " " + "findbyid(int Id)");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("{");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("try");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("{");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText(Environment.NewLine);
        //    if (i.EndsWith("y"))
        //    {
        //        var newi = i.Replace("y", "ies");
        //        //  txtoutput.AppendText("_dbcontext." + newi + "." + "Add(insert);");
        //        txtoutput.AppendText("var data = (from _" + i + " " + "in _repository._entity." + i + " " + "where _" + i + "." + i + "Id==Id");
        //    }
        //    else if (i.EndsWith("s"))
        //    {
        //        txtoutput.AppendText("var data = (from _" + i + " " + "in _repository._entity." + i + "es" + " " + "where _" + i + "." + i + "Id==Id");

        //    }
        //    else if (i.EndsWith("sh"))
        //    {

        //        txtoutput.AppendText("var data = (from _" + i + " " + "in _repository._entity." + i + "es" + " " + "where _" + i + "." + i + "Id==Id");

        //    }
        //    else if (i.EndsWith("ch"))
        //    {
        //        txtoutput.AppendText("var data = (from _" + i + " " + "in _repository._entity." + i + "es" + " " + "where _" + i + "." + i + "Id==Id");

        //    }
        //    else
        //    {
        //        txtoutput.AppendText("var data = (from _" + i + " " + "in _repository._entity." + i + "s" + " " + "where _" + i + "." + i + "Id==Id");

        //    }
        //    txtoutput.AppendText(Environment.NewLine);

        //    txtoutput.AppendText("select new " + i + "Contract");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("{");
        //    //gettableloop(i);
        //    foreach (var li in list)
        //    {
        //        txtoutput.AppendText(Environment.NewLine);
        //        if (li.datatype == "int" && li.isnull == "YES")
        //        {
        //            txtoutput.AppendText(Environment.NewLine);
        //            txtoutput.AppendText(li.columnname + "=_" + i + "." + li.columnname + ".Value,");
        //        }
        //        else
        //        {
        //            txtoutput.AppendText(Environment.NewLine);
        //            txtoutput.AppendText(li.columnname + "=_" + i + "." + li.columnname + ",");
        //        }
        //    }
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("}).SingleOrDefault();");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("return data;");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("}");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("catch(Exception ex)");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("{");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("//*** call your write to error log method or do what ever u like with the exception");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("return null;");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("}");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("}");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText(Environment.NewLine);
        //    //txtoutput.AppendText("public void Save()");
        //    //txtoutput.AppendText(Environment.NewLine);
        //    //txtoutput.AppendText("{");
        //    //txtoutput.AppendText(Environment.NewLine);
        //    //txtoutput.AppendText("_dbcontext.SaveChanges();");
        //    //txtoutput.AppendText(Environment.NewLine);

        //    //txtoutput.AppendText("_dbcontext.Dispose();");
        //    //txtoutput.AppendText(Environment.NewLine);
        //    //txtoutput.AppendText("}");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("public int Insert(" + i + "Contract" + " " + "model)");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("{");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("try");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("{");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("var insert = new Ef" + "." + i.ToString());
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("{");
        //    foreach (var li in list)
        //    {

        //        if (li.datatype == "int" && li.isnull == "YES")
        //        {
        //            txtoutput.AppendText(Environment.NewLine);
        //            txtoutput.AppendText(li.columnname + "=model." + li.columnname + ".Value,");
        //        }
        //        else
        //        {

        //            txtoutput.AppendText(Environment.NewLine);
        //            txtoutput.AppendText(li.columnname + "=model." + li.columnname + ",");
        //        }
        //    }

        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("};");
        //    txtoutput.AppendText(Environment.NewLine);
        //    if (i.EndsWith("y"))
        //    {
        //        var newi = i.Replace("y", "ies");
        //        txtoutput.AppendText("_repository._entity." + i + "." + "Add(insert);");
        //    }
        //    else if (i.EndsWith("s"))
        //    {
        //        txtoutput.AppendText("_repository._entity." + i + "." + "Add(insert);");
        //    }
        //    else if (i.EndsWith("ch"))
        //    {
        //        txtoutput.AppendText("_repository._entity." + i + "es" + "." + "Add(insert);");
        //    }
        //    else if (i.EndsWith("sh"))
        //    {
        //        txtoutput.AppendText("_repository._entity." + i + "es" + "." + "Add(insert);");
        //    }
        //    else
        //    {
        //        txtoutput.AppendText("_repository._entity." + i + "s" + "." + "Add(insert);");
        //    }

        //    txtoutput.AppendText(Environment.NewLine);
        //    //txtoutput.AppendText("Save();");
        //    txtoutput.AppendText(" _repository.Save();");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("return 1;");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("}");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("catch(Exception ex)");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("{");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("return 0;");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("//*** call your write to error log method or do what ever u like with the exception");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("}");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("}");

        //    txtoutput.AppendText("public bool Update(" + i + "Contract" + " " + "model)");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("{");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("try");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("{");
        //    txtoutput.AppendText("var update = new Ef" + "." + i.ToString());
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("{");
        //    foreach (var li in list)
        //    {
        //        txtoutput.AppendText(Environment.NewLine);
        //        txtoutput.AppendText(li.columnname + "=model." + li.columnname + ",");
        //        txtoutput.AppendText(Environment.NewLine);
        //    }
        //    list.Clear();
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("};");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("_repository._entity.Entry(update).State=System.Data.Entity.EntityState.Modified;");
        //    txtoutput.AppendText("//* please make sure you have imported the System.Data.Entity Namespace here and have refrenced the assembly**");
        //    txtoutput.AppendText(Environment.NewLine);
        //    //txtoutput.AppendText("Save();");

        //    txtoutput.AppendText(" _repository.Save();");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("return true;");
        //    txtoutput.AppendText(Environment.NewLine);

        //    txtoutput.AppendText("}");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("catch(Exception ex)");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("{");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("//*** call your write to error log method or do what ever u like with the exception");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("return false;");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("}");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("}");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("public int Delete(int id)");
        //    txtoutput.AppendText(Environment.NewLine);

        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("{");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("try{");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText(Environment.NewLine);
        //    if (i.EndsWith("y"))
        //    {
        //        var newi = i.Replace("y", "ies");
        //        txtoutput.AppendText(Environment.NewLine);
        //        txtoutput.AppendText("var delete = (from _" + i + " " + "in  _repository._entity." + i + " " + "where _" + i + "." + i + "Id==id");
        //        txtoutput.AppendText(Environment.NewLine);
        //        txtoutput.AppendText("select _" + i + ").SingleOrDefault(); ");
        //    }

        //    else if (i.EndsWith("sh"))
        //    {
        //        txtoutput.AppendText(Environment.NewLine);
        //        txtoutput.AppendText("var delete = (from _" + i + " " + "in  _repository._entity." + i + "es" + " " + "where _" + i + "." + i + "Id==" + i + "id");
        //        txtoutput.AppendText(Environment.NewLine);
        //        txtoutput.AppendText("select _" + i + ").SingleOrDefault(); ");
        //    }
        //    else if (i.EndsWith("ch"))
        //    {
        //        txtoutput.AppendText(Environment.NewLine);
        //        txtoutput.AppendText("var delete = (from _" + i + " " + "in  _repository._entity." + i + "es" + " " + "where _" + i + "." + i + "Id==" + i + "id");
        //        txtoutput.AppendText(Environment.NewLine);
        //        txtoutput.AppendText("select _" + i + ").SingleOrDefault(); ");
        //    }
        //    else
        //    {
        //        txtoutput.AppendText(Environment.NewLine);
        //        txtoutput.AppendText("var delete = (from _" + i + " " + "in _repository._entity" + i + "s" + " " + "where _" + i + "." + i + "Id==" + i + "id");
        //        txtoutput.AppendText(Environment.NewLine);
        //        txtoutput.AppendText("select _" + i + ").SingleOrDefault(); ");
        //    }
        //    txtoutput.AppendText(Environment.NewLine);

        //    txtoutput.AppendText(Environment.NewLine);
        //    if (i.EndsWith("y"))
        //    {
        //        var newi = i.Replace("y", "ies");
        //        txtoutput.AppendText("_repository._entity." + newi + "." + "Remove(delete);");
        //    }
        //    else if (i.EndsWith("sh"))
        //    {
        //        txtoutput.AppendText("_repository._entity." + i + "es" + "." + "Remove(delete);");

        //    }
        //    else if (i.EndsWith("ch"))
        //    {
        //        txtoutput.AppendText("_repository._entity." + i + "es" + "." + "Remove(delete);");

        //    }
        //    else if (i.EndsWith("s"))
        //    {
        //        txtoutput.AppendText("_repository._entity." + i + "." + "Remove(delete);");
        //    }

        //    else
        //    {
        //        txtoutput.AppendText("_repository._entity." + i + "s" + "." + "Remove(delete);");
        //    }

        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText(" _repository.Save();");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("return 1;");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("}");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("catch (Exception ex)");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("{");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("//*** call your write to error log method or do what ever u like with the exception");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("return 0;");
        //    txtoutput.AppendText("}");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("}");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("}");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("}");
        //    txtoutput.AppendText(Environment.NewLine);
        //    txtoutput.AppendText("//These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team //***");
        //}

        private void GenerateRepositories(string i)
        {
            txtoutput.AppendText("using System;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Collections.Generic;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.ComponentModel;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Data;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Linq;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Linq.Expressions;");

            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using" + " " + txtprojectname.Text + "." + "Infrastructure;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using" + " " + txtprojectname.Text + "." + "Ef;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using" + " " + txtprojectname.Text + "." + "DataContracts;");
            txtoutput.AppendText(Environment.NewLine);
            //txtoutput.AppendText("using System.ComponentModel.DataAnnotations;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("namespace" + " " + txtprojectname.Text + "." + "Repositories");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team ");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//*** Template Owner:Okonta Anthony | Email:okontaa@gmail.com | Mobile Number:+2348039436123....");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            //   txtoutput.AppendText("public partial class " + " " + i + "Repository" + ":IRepository<" + i + "Contract" + ">");
            txtoutput.AppendText("public partial class " + " " + i + "Repository");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("private Uow<" + i + "> _repository;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public " + " " + i + "Repository()");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("_repository = new Uow<" + i + ">();      //*Make an instance to your unit of work class");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            //txtoutput.AppendText(txtEntitiesName.Text.Trim() + " " + "_dbcontext=new" + " " + txtEntitiesName.Text.Trim() + "();");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public List<" + i + "Contract" + ">GetAll" + i + "()");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);

            txtoutput.AppendText("var _items = new List<" + i + "Contract" + ">();");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("try");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("var all = _repository.GetAll();");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("foreach(var item in all)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(" var _all = new " + " " + i + "Contract");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            gettableloop(i);
            foreach (var li in list)
            {
                txtoutput.AppendText(Environment.NewLine);



                if (li.datatype == "int" && li.isnull == "YES")
                {

                    txtoutput.AppendText(li.columnname + "=" + "item." + li.columnname + ".HasValue?" + "item." + li.columnname + ".Value:0,");
                }
                else if (li.datatype == "bit" && li.isnull == "YES")
                {

                    txtoutput.AppendText(li.columnname + "=" + "item." + li.columnname + ".HasValue?" + "item." + li.columnname + ".Value:false,");
                }
                else if (li.datatype == "datetime" && li.isnull == "YES")
                {

                    txtoutput.AppendText(li.columnname + "=" + "item." + li.columnname + ".HasValue?" + "item." + li.columnname + ".Value:DateTime.Now,");
                }
                else if (li.datatype == "long" && li.isnull == "YES")
                {

                    txtoutput.AppendText(li.columnname + "=" + "item." + li.columnname + ".HasValue?" + "item."+li.columnname + ".Value:0,");
                }


                else
                {
                    txtoutput.AppendText(li.columnname + "=" + "item." + li.columnname + ",");
                }

                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText(Environment.NewLine);
            }
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("};");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("_items.Add(_all);");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(" return _items;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("catch(Exception ex)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//*** call your write to error log method or do what ever u like with the exception");
            if (chkerrlog.Checked == true)
            {
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText("var ErrorLog = new ErrorLogContract");
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText("{");
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText("Error=ex.InnerMessage.ToString(),");
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText("ErrDate=DateTime.Now.ToString()");
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText("};");
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText("var adderror = ErrorLogRepository.AddErrorLog(ErrorLog);");
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText("_repository.Save();");
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText(Environment.NewLine);
            }

            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return new List<" + i + "Contract" + ">();");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public" + " " + i + "Contract" + " " + "Get" + i + "(int id)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("try");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("var item=_repository.GetAllById(id);");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(" var _items = new" + " " + i + "Contract");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            foreach (var li in list)
            {
                txtoutput.AppendText(Environment.NewLine);



                if (li.datatype == "int" && li.isnull == "YES")
                {

                    txtoutput.AppendText(li.columnname + "=" + "item." + li.columnname + ".HasValue?" + "item." + li.columnname + ".Value:0,");
                }
                else if (li.datatype == "bit" && li.isnull == "YES")
                {

                    txtoutput.AppendText(li.columnname + "=" + "item." + li.columnname + ".HasValue?" + "item." + li.columnname + ".Value:false,");
                }
                else if (li.datatype == "datetime" && li.isnull == "YES")
                {

                    txtoutput.AppendText(li.columnname + "=" + "item." + li.columnname + ".HasValue?" + "item." + li.columnname + ".Value:DateTime.Now,");
                }
                else if (li.datatype == "long" && li.isnull == "YES")
                {

                    txtoutput.AppendText(li.columnname + "=" + "item." + li.columnname + ".HasValue?" + "item." + li.columnname + ".Value:0,");
                }


                else
                {
                    txtoutput.AppendText(li.columnname + "=" + "item." + li.columnname + ",");
                }


                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText(Environment.NewLine);
            }

            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("};");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return _items;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("catch(Exception ex)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//*** call your write to error log method or do what ever u like with the exception");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return null;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public int Add" + i + "(" + i + "Contract" + " " + "item)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("try");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("var Obj = new " + " " + i);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            foreach (var li in list)
            {
                txtoutput.AppendText(Environment.NewLine);



                if (li.datatype == "int" && li.isnull == "YES")
                {

                    txtoutput.AppendText(li.columnname + "=" + "item." + li.columnname + ".HasValue?" + "item." + li.columnname + ".Value:0,");
                }
                else if (li.datatype == "bit" && li.isnull == "YES")
                {

                    txtoutput.AppendText(li.columnname + "=" + "item." + li.columnname + ".HasValue?" + "item." + li.columnname + ".Value:false,");
                }
                else if (li.datatype == "datetime" && li.isnull == "YES")
                {

                    txtoutput.AppendText(li.columnname + "=" + "item." + li.columnname + ".HasValue?" + "item." + li.columnname + ".Value:DateTime.Now,");
                }
                else if (li.datatype == "long" && li.isnull == "YES")
                {

                    txtoutput.AppendText(li.columnname + "=" + "item." + li.columnname + ".HasValue?" + "item." + li.columnname + ".Value:0,");
                }


                else
                {
                    txtoutput.AppendText(li.columnname + "=" + "item." + li.columnname + ",");
                }


                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText(Environment.NewLine);
            }
            txtoutput.AppendText(Environment.NewLine);

            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("};");
            txtoutput.AppendText(Environment.NewLine);

            txtoutput.AppendText("_repository.Add(Obj);");
            txtoutput.AppendText(Environment.NewLine);
            //txtoutput.AppendText("Save();");
            txtoutput.AppendText(" _repository.Save();");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return 1;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("catch(Exception ex)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return 0;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//*** call your write to error log method or do what ever u like with the exception");
            if (chkerrlog.Checked == true)
            {
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText("var ErrorLog = new ErrorLogContract");
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText("{");
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText("Error=ex.InnerMessage.ToString(),");
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText("ErrDate=DateTime.Now.ToString()");
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText("};");
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText("var adderror = ErrorLogRepository.AddErrorLog(ErrorLog);");
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText("_repository.Save();");
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText(Environment.NewLine);
            }
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");

            //*********************************************************************************************Update 
            txtoutput.AppendText("public bool Update(" + i + "Contract" + " " + "item)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("try");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(" var _update = new" + " " + i);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            foreach (var li in list)
            {
                txtoutput.AppendText(Environment.NewLine);


                if (li.datatype == "int" && li.isnull == "YES")
                {

                    txtoutput.AppendText(li.columnname + "=" + "item." + li.columnname + ".HasValue?" + "item." + li.columnname + ".Value:0,");
                }
                else if (li.datatype == "bit" && li.isnull == "YES")
                {

                    txtoutput.AppendText(li.columnname + "=" + "item." + li.columnname + ".HasValue?" + "item." + li.columnname + ".Value:false,");
                }
                else if (li.datatype == "datetime" && li.isnull == "YES")
                {

                    txtoutput.AppendText(li.columnname + "=" + "item." + li.columnname + ".HasValue?" + "item." + li.columnname + ".Value:DateTime.Now,");
                }
                else if (li.datatype == "long" && li.isnull == "YES")
                {

                    txtoutput.AppendText(li.columnname + "=" + "item." + li.columnname + ".HasValue?" + "item." + li.columnname + ".Value:0,");
                }


                else
                {
                    txtoutput.AppendText(li.columnname + "=" + "item." + li.columnname + ",");
                }

                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText(Environment.NewLine);
            }

            txtoutput.AppendText("};");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("_repository.Update(_update);");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(" _repository.Save();");

            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return true;");

            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);


            list.Clear();
            //txtoutput.AppendText("Save();");

            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("  catch (Exception ex)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText("//*** call your write to error log method or do what ever u like with the exception");
            if (chkerrlog.Checked == true)
            {
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText("var ErrorLog = new ErrorLogContract");
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText("{");
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText("Error=ex.InnerMessage.ToString(),");
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText("ErrDate=DateTime.Now.ToString()");
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText("};");
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText("var adderror = ErrorLogRepository.AddErrorLog(ErrorLog);");
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText("_repository.Save();");
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText(Environment.NewLine);
            }
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return false;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);



            //**************************************************************************** GetAllByExpression

            //FindByExpressions(Expression<Func<T, bool>> predicate);");
            txtoutput.AppendText("public IQueryable< " + i + "Contract" + "> GetByExpression(Expression<Func<T, bool>> predicate)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("try");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("_repository.GetAllByExpression(predicate);");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("catch (Exception ex)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText("//*** call your write to error log method or do what ever u like with the exception");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);



            //****************************************************************************

            //*****************************************************************************


            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);

            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public int Delete(int id)");
            txtoutput.AppendText(Environment.NewLine);

            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("try{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(" var model = _repository.GetAllById(id);");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("_repository.Remove(model);");

            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(" _repository.Save();");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return 1;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("catch (Exception ex)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//*** call your write to error log method or do what ever u like with the exception");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return 0;");
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team //***");
        }

        private void GenerateCustomizedRepositories(string i)
        {

            txtoutput.AppendText("using System;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Collections.Generic;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.ComponentModel;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Data;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Linq;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using" + " " + txtprojectname.Text + "." + "Infrastructure;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using" + " " + txtprojectname.Text + "." + "Ef;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using" + " " + txtprojectname.Text + "." + "DataContracts;");
            txtoutput.AppendText(Environment.NewLine);
            //txtoutput.AppendText("using System.ComponentModel.DataAnnotations;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("namespace" + " " + txtprojectname.Text + "." + "Repositories");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team ");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//*** Template Owner:Okonta Anthony | Email:okontaa@gmail.com | Mobile Number:+2348039436123....");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public partial class " + " " + i + "Repository");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");

            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team //***");
        }

        public class check
        {
            public string datatype { get; set; }
            public string isnull { get; set; }
            public string columnname { get; set; }
        }

        public void GenerateIndexView(string i)
        {
            txtoutput.AppendText("@model IEnumerable<" + txtprojectname.Text + ".DataContracts." + i + "DataContracts>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("@{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("ViewBag.Title = " + "\"Index\";");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("Layout = " + "\"~/Views/Shared/_Layout.cshtml\";");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("<h2>" + i + " " + "List" + "</h2>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("<p>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("@Html.ActionLink(" + "\"Create New\"" + ", " + "\"Create\"" + ")");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("</p>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("<table>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("<tr>");
            gettableloop(i);
            foreach (var li in list)
            {
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText("<th>");
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText(" @Html.DisplayNameFor(model => model." + li.columnname + ")");
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText("</th>");
                txtoutput.AppendText(Environment.NewLine);
            }
            txtoutput.AppendText("</tr>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("@foreach (var item in Model) {");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("<tr>");
            foreach (var li in list)
            {
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText("<td>");
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText(" @Html.DisplayNameFor(model => model." + li.columnname + ")");
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText("</td>");
                txtoutput.AppendText(Environment.NewLine);
            }
            // list.Clear();
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("<td>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(" @Html.ActionLink(" + "\"Edit\"" + "," + "\"Edit\"" + "," + "new { id=item." + i + "Id}) |");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(" @Html.ActionLink(" + "\"Details\"" + "," + "\"Details\"" + "," + "new { id=item." + i + "Id}) |");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(" @Html.ActionLink(" + "\"Delete\"" + "," + "\"Delete\"" + "," + "new { id=item." + i + "Id}) |");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("</td>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("</tr>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("</table>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("@*Template Owner:Okonta Anthony | Email:okontaa@gmail.com | Mobile Number:+2348039436123....*@");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("@*These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team *@");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
        }

        public void GenerateCreateView(string i)
        {
            txtoutput.AppendText("@model " + txtprojectname.Text + ".DataContracts." + i);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("@{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("ViewBag.Title = " + "\"Create\";");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("Layout = " + "\"~/Views/Shared/_Layout.cshtml\";");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("<h2> Create new " + " " + i + "</h2>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("@using (Html.BeginForm(@*You can Specify your Own ActionName and Controlle with FormMethod*@)) {");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(" @Html.AntiForgeryToken()");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(" @Html.ValidationSummary(true)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("<fieldset>");
            txtoutput.AppendText(Environment.NewLine);

            txtoutput.AppendText(Environment.NewLine);
            // gettableloop(i);

            foreach (var li in list)
            {
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText("<div class=" + "editor-label>" + "");
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText(" @Html.LabelFor(model => model." + li.columnname + ")");
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText("</div>");
                txtoutput.AppendText(Environment.NewLine);

                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText("<div class=" + "editor-field>" + "");
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText(" @Html.TextBoxFor(model => model." + li.columnname + ")");
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText(" @Html.ValidationMessageFor(model => model." + li.columnname + ")");
                //txtoutput.AppendText("@Html.ValidationMessageFor(model => model.FirstName)");
                txtoutput.AppendText(Environment.NewLine);
                txtoutput.AppendText("</div>");
                txtoutput.AppendText(Environment.NewLine);
            }
            //  list.Clear();
            txtoutput.AppendText("<p>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("<input type=" + "submit" + " " + "value=" + "Create" + "/>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("</p>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("</fieldset>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("<div>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("@Html.ActionLink(" + "\"Back to List\"" + "," + "\"Index\"" + ")");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("</div>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("@section Scripts {");

            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("  @Scripts.Render(" + "\"~/bundles/jqueryval\"" + ")");
            txtoutput.AppendText(Environment.NewLine);

            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("@*Template Owner:Okonta Anthony | Email:okontaa@gmail.com | Mobile Number:+2348039436123....*@");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("@*These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team *@");
            txtoutput.AppendText(Environment.NewLine);
        }

        public void GenerateEditView(string i)
        {
            txtoutput.AppendText("@model " + txtprojectname.Text + ".DataContracts." + i);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("@{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("ViewBag.Title = " + "\"Edit\";");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("Layout = " + "\"~/Views/Shared/_Layout.cshtml\";");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("<h2> Edit " + " " + i + "</h2>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("@using (Html.BeginForm(@*You can Specify your Own ActionName and Controlle with FormMethod*@)) {");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(" @Html.AntiForgeryToken()");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(" @Html.ValidationSummary(true)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("<fieldset>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("<legend>" + i + "</legend>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("@Html.HiddenFor(model => model." + i + "Id)");
            txtoutput.AppendText(Environment.NewLine);
            // gettableloop(i);
            foreach (var li in list)
            {
                if (li.columnname.Contains(i + "Id"))
                {
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("@*<div class=" + "editor-label>" + "");
                    txtoutput.AppendText(Environment.NewLine);

                    txtoutput.AppendText(" @Html.LabelFor(model => model." + li.columnname + ")");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("</div>");
                    txtoutput.AppendText(Environment.NewLine);

                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("<div class=" + "editor-field>" + "");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(" @Html.TextBoxFor(model => model." + li.columnname + ")");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(" @Html.ValidationMessageFor(model => model." + li.columnname + ")");
                    //txtoutput.AppendText("@Html.ValidationMessageFor(model => model.FirstName)");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("</div>*@");
                    txtoutput.AppendText(Environment.NewLine);

                }
                else
                {
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("<div class=" + "editor-label>" + "");
                    txtoutput.AppendText(Environment.NewLine);

                    txtoutput.AppendText(" @Html.LabelFor(model => model." + li.columnname + ")");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("</div>");
                    txtoutput.AppendText(Environment.NewLine);

                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("<div class=" + "editor-field>" + "");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(" @Html.TextBoxFor(model => model." + li.columnname + ")");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(" @Html.ValidationMessageFor(model => model." + li.columnname + ")");
                    //txtoutput.AppendText("@Html.ValidationMessageFor(model => model.FirstName)");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("</div>");
                    txtoutput.AppendText(Environment.NewLine);
                }
            }
            // list.Clear();
            txtoutput.AppendText("<p>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("<input type=" + "submit" + " " + "value=" + "Update" + "/>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("</p>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("</fieldset>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("<div>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("@Html.ActionLink(" + "\"Back to List\"" + "," + "\"Index\"" + ")");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("</div>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("@section Scripts {");

            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("  @Scripts.Render(" + "\"~/bundles/jqueryval\"" + ")");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("@*Template Owner:Okonta Anthony | Email:okontaa@gmail.com | Mobile Number:+2348039436123....*@");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("@*These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team *@");
            txtoutput.AppendText(Environment.NewLine);
        }

        public void GenerateDetailsView(string i)
        {
            txtoutput.AppendText("@model " + txtprojectname.Text + ".DataContracts." + i);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("@{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("ViewBag.Title = " + "\"Details\";");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("Layout = " + "\"~/Views/Shared/_Layout.cshtml\";");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            // txtoutput.AppendText("<h2> Edit " + " " + i + "</h2>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("@using (Html.BeginForm(@*You can Specify your Own ActionName and Controlle with FormMethod*@)) {");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(" @Html.AntiForgeryToken()");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(" @Html.ValidationSummary(true)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("<fieldset>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("<legend>" + i + "</legend>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("@Html.HiddenFor(model => model." + i + "Id)");
            txtoutput.AppendText(Environment.NewLine);
            // gettableloop(i);
            foreach (var li in list)
            {
                if (li.columnname.Contains(i + "Id"))
                {
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("@*<div class=" + "editor-label>" + "");
                    txtoutput.AppendText(Environment.NewLine);

                    txtoutput.AppendText(" @Html.LabelFor(model => model." + li.columnname + ")");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("</div>");
                    txtoutput.AppendText(Environment.NewLine);

                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("<div class=" + "editor-field>" + "");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(" @Html.TextBoxFor(model => model." + li.columnname + ")");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(" @Html.ValidationMessageFor(model => model." + li.columnname + ")");
                    //txtoutput.AppendText("@Html.ValidationMessageFor(model => model.FirstName)");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("</div>*@");
                    txtoutput.AppendText(Environment.NewLine);

                }
                else
                {
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("<div class=" + "editor-label>" + "");
                    txtoutput.AppendText(Environment.NewLine);

                    txtoutput.AppendText(" @Html.LabelFor(model => model." + li.columnname + ")");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("</div>");
                    txtoutput.AppendText(Environment.NewLine);

                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("<div class=" + "editor-field>" + "");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(" @Html.TextBoxFor(model => model." + li.columnname + ")");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText(" @Html.ValidationMessageFor(model => model." + li.columnname + ")");
                    //txtoutput.AppendText("@Html.ValidationMessageFor(model => model.FirstName)");
                    txtoutput.AppendText(Environment.NewLine);
                    txtoutput.AppendText("</div>");
                    txtoutput.AppendText(Environment.NewLine);
                }
            }
            // list.Clear();
            txtoutput.AppendText("<p>");
            txtoutput.AppendText(Environment.NewLine);
            //txtoutput.AppendText("<input type=" + "submit" + " " + "value=" + "Update" + "/>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("</p>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("</fieldset>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("<div>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("@Html.ActionLink(" + "\"Back to List\"" + "," + "\"Index\"" + ")");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("</div>");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("@section Scripts {");

            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("  @Scripts.Render(" + "\"~/bundles/jqueryval\"" + ")");
            txtoutput.AppendText(Environment.NewLine);

            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("@*Template Owner:Okonta Anthony | Email:okontaa@gmail.com | Mobile Number:+2348039436123....*@");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("@*These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team *@");
            txtoutput.AppendText(Environment.NewLine);
        }

        public void GenerateControllers(string i)
        {

            txtoutput.AppendText("using System;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Collections.Generic;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Linq;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Web;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Web.Mvc;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using" + " " + txtprojectname.Text + "." + "DataContracts;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using" + " " + txtprojectname.Text + "." + "Services;");
            txtoutput.AppendText(Environment.NewLine);

            txtoutput.AppendText(Environment.NewLine);

            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("namespace" + " " + txtprojectname.Text + "." + "Controllers");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team ");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//*** Template Owner:Okonta Anthony | Email:okontaa@gmail.com | Mobile Number:+2348039436123....");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public class " + i + "Controller : Controller");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("// GET: /" + i + "/" + "");
            txtoutput.AppendText(Environment.NewLine);
            //txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public ActionResult Index()");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return View();");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("// GET: /" + i + "/Details/5" + "");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public ActionResult Details(int id)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return View();");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("// GET: /" + i + "/Create/" + "");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public ActionResult Create()");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return View();");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("// Post: /" + i + "/Create" + "");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("[HttpPost]");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public ActionResult Create(" + i + "Contract" + " " + "model)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);

            txtoutput.AppendText("try");
            txtoutput.AppendText(Environment.NewLine);

            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("// TODO: Add insert logic here");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return RedirectToAction(" + "\"Index\");");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("catch(Exception Ex)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("// you can log the Exception here by calling your log error function");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("ModelState.AddModelError(" + "\" \", " + "\"An Error Occured While Inserting \");");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return View();");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);


            //**********************************************************************************************

            txtoutput.AppendText("// GET: /" + i + "/Edit/5" + "");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public ActionResult Edit(int id)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return View();");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("// Post: /" + i + "/Edit/5" + "");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("[HttpPost]");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public ActionResult Edit(" + i + "Contract" + " " + "model)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);

            txtoutput.AppendText("try");
            txtoutput.AppendText(Environment.NewLine);

            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("// TODO: Add Update logic here");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return RedirectToAction(" + "\"Index\");");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("catch(Exception Ex)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("// you can log the Exception here by calling your log error function");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("ModelState.AddModelError(" + "\" \", " + "\"An Error Occured While Updating \");");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return View();");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);


            //*************************************************************************************************
            //Method to generate Delete Action.
            //*************************************************************************************************
            txtoutput.AppendText("// GET: /" + i + "/Delete/5" + "");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public ActionResult Delete(int id)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return View();");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("// Post: /" + i + "/Delete" + "");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("[HttpPost]");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public ActionResult Delete(" + i + "Contract" + " " + "model)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);

            txtoutput.AppendText("try");
            txtoutput.AppendText(Environment.NewLine);

            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("// TODO: Add Delete logic here");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return RedirectToAction(" + "\"Index\");");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("catch(Exception Ex)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("// you can log the Exception here by calling your log error function");
            txtoutput.AppendText(Environment.NewLine);

            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("ModelState.AddModelError(" + "\" \", " + "\"An Error Occured While Deleting The Record \");");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return View();");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team //***");
        }

        public void CheckForeignKey(string i)
        {
            var con = txtConnectionString.Text;
            var cn = new SqlConnection(con + "Initial catalog=" + checkedListBox1.SelectedItem);
            cn.Open();

            var cmd = new SqlCommand("sp_fkeys", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@pktable_name", i);

            rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                tablename = rdr["FKTABLE_NAME"].ToString();
                fkname = rdr["FKTABLE_NAME"].ToString();

            }
        }

        //*******************************************************************************************

        //Method Below is to create IUnit of Work

        //*******************************************************************************************


        public void GenerateUnitofwork()
        {
            txtoutput.AppendText("using System;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Collections.Generic;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.ComponentModel;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Data.Entity;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Data;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Linq;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using" + " " + txtprojectname.Text + "." + "Ef;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            //txtoutput.AppendText("using System.ComponentModel.DataAnnotations;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("namespace" + " " + txtprojectname.Text + "." + "Infrastructure");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team ");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//*** Template Owner:Okonta Anthony | Email:okontaa@gmail.com | Mobile Number:+2348039436123....");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public partial class Uow<T>:IRepository<T>,IUnitow<T> where T:class");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("private " + txtEntitiesName.Text.ToString().Trim() + " " + "_entity;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("private readonly DbSet<T>_dbset;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public Uow ()");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("_entity= new " + " " + txtEntitiesName.Text.ToString().Trim() + "();");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("_dbset=_entity.Set<T>();");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("_entity.Configuration.LazyLoadingEnabled=true;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public void Save()");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("var _transaction =_entity.Database.BeginTransaction();");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("if(_transaction!=null)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("_entity.SaveChanges();");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("_transaction.Commit();");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("Dispose();");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("else");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("_transaction.Rollback();");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public void Dispose()");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("_entity.Dispose();");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(" public void Add(T model)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("_dbset.Add(model);");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(" public void Remove(T model)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("_dbset.Remove(model);");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(" public void Update(T model)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            // _entities.Entry(entity).State = System.Data.EntityState.Modified;
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("_entity.Entry(model).State = EntityState.Modified;   //******* please make sure you have imported the System.Data.Entity Namespace here and have refrenced the assembly**");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");


            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(" public IEnumerable<T> GetAll()");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return _dbset.ToList<T>();");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");

            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public T GetAllById(int id)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return _dbset.Find(id);");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");


            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public T FindByExpressions(System.Linq.Expressions.Expression<Func<T, bool>> predicate)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return _dbset.Where(predicate);");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");


            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public IQueryable<T> FindAllByExpressions(System.Linq.Expressions.Expression<Func<T, bool>> predicate)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return _dbset.Where(predicate).ToList<T>();");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");

            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team //***");
        }



        private void btnbrowsepath_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            txtscriptoutput.Text = folderBrowserDialog1.SelectedPath;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtactivity.Text = "";
        }

        private void btnclearDatabase_Click(object sender, EventArgs e)
        {
            checkedListBox1.Items.Clear();
        }


        public void GenerateErrorLog(string DatabaseName)
        {
            var con = txtConnectionString.Text;
            var cn = new SqlConnection(con + "Initial catalog=" + checkedListBox1.SelectedItem);
            cn.Open();
            string sql1 = "use" + checkedListBox1.SelectedItem;// COLUMN_NAME,DATA_TYPE,IS_NULLABLE from INFORMATION_SCHEMA.COLUMNS IC where TABLE_NAME=@id";
            string sql2 = "create table ErrorLog(ErrorLogId int identity(1,1) Primary Key,Error nvarchar(4000),ErrDate datetime)";
            var cmd = new SqlCommand(sql1, cn);
            cmd = new SqlCommand(sql2, cn);
            cmd.ExecuteNonQuery();
        }



        public void GenerateHelpers()
        {
            txtoutput.AppendText("using System;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Collections.Generic;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.ComponentModel;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Data;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Linq;");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("using System.Web;");

            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("namespace" + " " + txtprojectname.Text + "." + "Infrastructure");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team ");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//*** Template Owner:Okonta Anthony | Email:okontaa@gmail.com | Mobile Number:+2348039436123....");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public class helper");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public const string Username=" + "\"username\";");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public const string Role=" + "\"role\";");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public const string Branch=" + "\"branch\";");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public const string Department=" + "\"dept\";");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public const string Sol=" + "\"sol\";");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public const string IpAddress=" + "\"ipaddress\";");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public const string MacAddress=" + "\"macaddress\";");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public const string MachineName=" + "\"machinename\";");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public static string GetUserName");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("get{var item = HttpContext.Current.Session[" + "\"Username\"" + "];");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("if(item!=null){return item.ToString();}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("else");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{return null;}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("set{HttpContext.Current.Session[Username] = value;}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
           
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public  string GetRole");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("get{var item = HttpContext.Current.Session[" + "\"Role\"" + "];");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("if(item!=null){return item.ToString();}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("else");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{return null;}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("set{HttpContext.Current.Session[Role] = value;}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public  string GetBranch");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("get{var item = HttpContext.Current.Session[" + "\"Branch\"" + "];");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("if(item!=null){return item.ToString();}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("else");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{return null;}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("set{HttpContext.Current.Session[Branch] = value;}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public  string GetDepartment");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("get{var item = HttpContext.Current.Session[" + "\"Department\"" + "];");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("if(item!=null){return item.ToString();}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("else");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{return null;}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("set{HttpContext.Current.Session[Department] = value;}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public  string GetSol");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("get{var item = HttpContext.Current.Session[" + "\"Sol\"" + "];");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("if(item!=null){return item.ToString();}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("else");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{return null;}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("set{HttpContext.Current.Session[Sol] = value;}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public  string GetIpAddress");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("get{var item = HttpContext.Current.Session[" + "\"IpAddress\"" + "];");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("if(item!=null){return item.ToString();}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("else");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{return null;}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("set{HttpContext.Current.Session[IpAddress] = value;}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public  string GetMachineName");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("get{var item = HttpContext.Current.Session[" + "\"MachineName\"" + "];");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("if(item!=null){return item.ToString();}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("else");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{return null;}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("set{HttpContext.Current.Session[MachineName] = value;}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public  string GetMacAddress");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("get{var item = HttpContext.Current.Session[" + "\"MacAddress\"" + "];");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("if(item!=null){return item.ToString();}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("else");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{return null;}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("set{HttpContext.Current.Session[MacAddress] = value;}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("public string EncryptText(string text)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("var secretBytes = Encoding.Unicode.GetBytes(text.Trim())");

            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("var encryptedBytes = ProtectedData.Protect(secretBytes, null, DataProtectionScope.LocalMachine);");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return Encoding.Unicode.GetString(encryptedBytes).ToString();");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);

            txtoutput.AppendText("public string DecryptText(string text)");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("{");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText(" var decryptedBytes = ProtectedData.Unprotect(text, null, DataProtectionScope.LocalMachine);");

            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("var decryptedSecret = Encoding.Unicode.GetString(decryptedBytes);");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("return decryptedSecret.ToString();");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);

            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("}");
            txtoutput.AppendText(Environment.NewLine);
            txtoutput.AppendText("//*These codes were generated by EntityGenerator on " + DateTime.Today.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "Happy Coding....Cheers The Infinite Solution Team *");
            txtoutput.AppendText(Environment.NewLine);
        }

    }
}
