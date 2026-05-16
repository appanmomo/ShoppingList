using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ShoppingList.Models;

namespace ShoppingList.Views;

public partial class NewAccountPage : ContentPage
{
    public NewAccountPage()
    {
        InitializeComponent();
        Title = "Create New Account";
    }

    async void CreateAccount_OnClicked(object sender, EventArgs e)
    {
        // null field checks
        if (string.IsNullOrWhiteSpace(txtUser.Text))
        {
            await DisplayAlert("Error", "Username required", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(txtPassword1.Text))
        {
            await DisplayAlert("Error", "Password required", "OK");
            return;
        }
        
        if (string.IsNullOrWhiteSpace(txtEmail.Text))
        {
            await DisplayAlert("Error", "Please fill in all fields.", "OK");
            return;
        }
        
        
        // min length checks 
        if (txtUser.Text.Length < 3)
        {
            await DisplayAlert("Error", "Username must contain at least 3 characters.", "OK");
            return;
        }

        if (txtPassword1.Text.Length < 6)
        {
            await DisplayAlert("Error", "Password must contain at least 6 characters.", "OK");
            return;
        }
        
        // password match check
        if (txtPassword1.Text != txtPassword2.Text)
        {
            await DisplayAlert("Error", "Passwords do not match.", "OK");
            return;
        }
        
        // valid email check = ( @ and .)
        string email = txtEmail.Text;

        if (!email.Contains("@"))
        {
            await DisplayAlert("Error", "Email must contain @", "OK");
            return;
        }

        if (!email.Contains("."))
        {
            await DisplayAlert("Error", "Email must contain .", "OK");
            return;
        }
        
        if (email.IndexOf("@") > email.LastIndexOf("."))
        {
            await DisplayAlert("Error", "Please enter a valid email address.", "OK");
            return;
        }
        
        // api stuff
        var data = JsonConvert.SerializeObject(new UserAccount(txtUser.Text, txtPassword1.Text, txtEmail.Text));

        var client = new HttpClient();
        var response = await client.PostAsync(new Uri("https://joewetzel.com/fvtc/account/createuser"),new StringContent(data, Encoding.UTF8, "application/json"));

        var AccountStatus = response.Content.ReadAsStringAsync().Result;
        
        // user exists check 
        if (AccountStatus=="user exists")
        {
            await DisplayAlert("Error", "This username has been taken.", "OK");
            return;
        }
            
            
        // has email been used check
        if (AccountStatus=="email exists")
        {
            await DisplayAlert("Error", "This email is already associated with an existing account.", "OK");
            return;
        }
        
        if (AccountStatus=="complete")
        {
            
            response = await client.PostAsync(new Uri("https://joewetzel.com/fvtc/account/login"),new StringContent(data, Encoding.UTF8, "application/json"));

            var SKey = response.Content.ReadAsStringAsync().Result;

            if (!string.IsNullOrEmpty(SKey) || SKey.Length > 50 )
            {
            App.SessionKey = SKey;
            Navigation.PopModalAsync();
            }
            else
            {
                await DisplayAlert("Error", "Sorry, an error has occurred.", "OK");
                return; 
            }
            

        }
        else
        {
            await DisplayAlert("Error", "Sorry, there was an error creating your account.", "OK");
            return;
        }
        
    }
}