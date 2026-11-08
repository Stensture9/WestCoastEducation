using CommunityToolkit.Mvvm.ComponentModel;
using WestCoastEducation.Models;

namespace WestCoastEducation.ViewModels;

public abstract partial class PersonFormViewModelBase : ViewModelBase
{
    [ObservableProperty] private string _firstName = "";
    [ObservableProperty] private string _lastName = "";
    [ObservableProperty] private string _phone = "";
    [ObservableProperty] private string _personalNumber = "";
    [ObservableProperty] private string _street = "";
    [ObservableProperty] private string _postalCode = "";
    [ObservableProperty] private string _city = "";

    protected T BuildPerson<T>(string? id) where T : Person, new() => new()
    {
        Id = id,
        FirstName = FirstName.Trim(),
        LastName = LastName.Trim(),
        Phone = Phone.Trim(),
        PersonalNumber = PersonalNumber.Trim(),
        Address = new Address
        {
            Street = Street.Trim(),
            PostalCode = PostalCode.Trim(),
            City = City.Trim()
        }
    };

    protected void FillFrom(Person person)
    {
        FirstName = person.FirstName;
        LastName = person.LastName;
        Phone = person.Phone;
        PersonalNumber = person.PersonalNumber;
        Street = person.Address.Street;
        PostalCode = person.Address.PostalCode;
        City = person.Address.City;
    }

    protected void ClearPersonFields()
    {
        FirstName = LastName = Phone = PersonalNumber = "";
        Street = PostalCode = City = "";
    }
}
