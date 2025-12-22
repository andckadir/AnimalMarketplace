// using AnimalMarketplace.Models;
//
// namespace AnimalMarketplace.Database.DTO;
//
// public class AnimalResponseDto
// {
//     public int Age { get; set; }
//     public Gender Gender { get; set; }
//     public AnimalKind Kind { get; set; }
//     public string SellerBusinessName { get; set; }
//     public string SellerPhone { get; set; }
//     public AnimalResponseDto(Animal animal)
//     {
//         Age = animal.Age;
//         Gender = animal.Gender;
//         Kind = animal.Kind;
//         SellerBusinessName = animal.Advert.Seller.BusinessName; // Null-safe
//         SellerSurname = animal.Seller.Surname;
//         SellerPhone = animal.Seller.Phone;
//         
//     }
// }