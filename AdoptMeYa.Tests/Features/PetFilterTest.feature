Feature: PetFilterTest
As a User
I want to apply filters in pet search
So I can find faster a pet I want

	Background: 
		Given The endpoint https://localhost:5001/api/v1/Pets is available
		And A User is already Stored
		  | Id   | Type | UserNick | Pass   | Ruc      | Dni      | Phone     | Email             | Name      | LastName | DistrictId |
		  | 1000 | VET  | string   | string | A12345rf | 70258688 | 946401234 | frank@outlook.com | Francisco | Voularte | 0          |
		And A pet is already stored
		  | Id | Type | Name | Attention | Race    | Age | Gender | IsAdopted | UserId | PublicationId |
		  | 1  | Can  | Lolo | Yes       | Pitbull | 2   | Female | false     | 1000   | 1             |
  
  
@pet-filter
Scenario: Search by Type of existing Pet
	Given the endpoint https://localhost:5001/api/v1/Pets/type=Can is available
	When a get request with filters is sent
	Then A Pet Resource is included in Response Body with one Filter
	  | Id | Type | Name | Attention | Race    | Age | Gender | IsAdopted | UserId | PublicationId |
	  | 1  | Can  | Lolo | Yes       | Pitbull | 2   | Female | false     | 1000   | 1             |
   
	Scenario: Search by Type of non existing Pet
		Given the endpoint https://localhost:5001/api/v1/Pets/type=Cans is available
		When a get request with filters is sent
		Then An empty list is included in Response Body
		
	Scenario: Search by Type Gender and Attention of existing Pet
		Given the Endpoint https://localhost:5001/api/v1/Pets/type=Can/gender=Female/attention=Yes is available
		When a get request with filters is sent
		Then A Pet Resource is included in Response Body with many Filters
		  | Id | Type | Name | Attention | Race    | Age | Gender | IsAdopted | UserId | PublicationId |
		  | 1  | Can  | Lolo | Yes       | Pitbull | 2   | Female | false     | 1000   | 1             |
    
	Scenario: Search by Type Gender and Attention of non existing Pet
		Given the Endpoint https://localhost:5001/api/v1/Pets/type=Can/gender=Male/attention=Yes is available
		When a get request with filters is sent
		Then An empty list is included in Response Body
