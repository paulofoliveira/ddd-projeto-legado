using PackageDeliveryNew.Common;
using System.Collections.Generic;

namespace PackageDeliveryNew.Deliveries
{
    public class Address : ValueObject<Address>
    {
        public Address(string street, string city, string state, string zipCode)
        {
            Contracts.Require(street != null);
            Contracts.Require(city != null);
            Contracts.Require(state != null);
            Contracts.Require(zipCode != null);

            Street = street;
            City = city;
            State = state;
            ZipCode = zipCode;
        }

        public string Street { get; }
        public string City { get; }
        public string State { get; }
        public string ZipCode { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Street;
            yield return City;
            yield return State;
            yield return ZipCode;
        }
    }
}