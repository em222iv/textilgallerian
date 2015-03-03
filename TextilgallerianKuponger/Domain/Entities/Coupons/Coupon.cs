﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Domain.Entities
{
    /// <summary>
    ///     Base class for discount coupons
    /// </summary>
    public abstract class Coupon
    {
        public virtual void SetProperties(IReadOnlyDictionary<string, string> properties)
        {
            Name = properties["Name"];
            Code = properties["Code"];
            Description = properties["Description"];
            Start = DateTime.Parse(properties["Start"]);
            End = properties["End"] == "" ? new DateTime?() : DateTime.Parse(properties["End"]);
            if (!String.IsNullOrWhiteSpace(properties["UseLimit"]))
            {
                UseLimit = int.Parse(properties["UseLimit"]);
            }
            MinPurchase = Decimal.Parse(properties["MinPurchase"], CultureInfo.InvariantCulture);
        }

        public virtual Dictionary<string, string> GetProperties()
        {
            return new Dictionary<String, String>
            {
                {"Name", Name},
                {"Code",  Code},
                {"Description", Description},
                {"Start", Start.ToString("yyyy-MM-dd")},
                {"End", End.HasValue ? End.Value.ToString("yyyy-MM-dd") : null},
                {"UseLimit", UseLimit.ToString()},
                {"MinPurchase", MinPurchase.ToString(CultureInfo.InvariantCulture)},
                {"UniqueKey", UniqueKey}
            };
        }

        /// <summary>
        ///     The code for this coupon
        ///     (auto-generated by default)
        /// </summary>
        public String Code { get; set; }

        /// <summary>
        /// Key that is unique for all coupons.
        /// </summary>
        public String UniqueKey { get; set; }

        public String Name { get; set; }
        public String Description { get; set; }

        /// <summary>
        ///     Do not ever use this anywhere else than in the API!
        /// </summary>
        public String Type
        {
            get { return GetType().Name; }
        }

        /// <summary>
        ///     When the coupon should start to be valid
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        ///     When the coupon should end
        ///     Optional, can last forever
        /// </summary>
        public DateTime? End { get; set; }

        /// <summary>
        ///     When the coupon where created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        ///     Which customers is this coupon valid for
        ///     If coupon is for all customers it should be set to null
        /// </summary>

        // TODO: Q: Should this list be emptied?!
        public List<Customer> CustomersValidFor { get; set; }

        /// <summary>
        ///     Which users have used this coupon
        /// </summary>

        // TODO: Q: Should CustomersValidFor be inserted here when used?!
        public List<Customer> CustomersUsedBy { get; set; }

        /// <summary>
        ///     Max amount of times that a customer is allowed to use the coupon
        /// </summary>
        public int? UseLimit { get; set; }

        /// <summary>
        ///  Property to see what user that created the coupon
        /// </summary>
        public String CreatedBy { get; set; }

        /// <summary>
        ///     Boolean deciding if the coupon can be combined
        ///     with other coupons
        /// </summary>
        public bool CanBeCombined { get; set; }

        /// <summary>
        ///     Determaines if the coupon is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        ///     Products valid for this discount
        /// </summary>
        public List<Product> Products { get; set; }

        ///// <summary>
        /////     How many products customer need to buy
        ///// </summary>
        //public Decimal Buy { get; set; }


        /// <summary>
        ///     The minimum purchase sum required for the coupon to be valid
        /// </summary>
        public Decimal MinPurchase { get; set; }

        /// <summary>
        ///     Check if specified Cart is valid for this Coupon
        /// </summary>

        // TODO: Needs refactoring and tests
        public virtual Boolean IsValidFor(Cart cart)
        {
<<<<<<< HEAD
            //not valid if End date has not passed, or if the minimum purchase limit has not been reached.
            if ((End.HasValue && End.Value < DateTime.Now) || MinPurchase > cart.TotalSum)
=======
            if (!IsActive)
            {
                return false;
            }

            if (Code != null && Code != cart.CouponCode)
            {
                return false;
            }

            if (End.HasValue && End.Value < DateTime.Now)
            {
                return false;
            }

            if (MinPurchase > cart.TotalSum)
>>>>>>> fix(Domain): Fixes domain logic; tested by calling API
            {
                return false;
            }

            if (CustomersValidFor != null)
            {
                var customer =
                    CustomersValidFor.Find(cust => (cust.CouponCode == cart.CouponCode && cust.CouponCode != null) ||
                                                   (cust.Email == cart.Customer.Email && cust.Email != null) ||
                                                   (cust.SocialSecurityNumber == cart.Customer.SocialSecurityNumber && cust.SocialSecurityNumber != null));
                return customer != null && customer.CouponUses < UseLimit;
            }
            
            if (UseLimit.HasValue && UseLimit.Value <= 0)
            {
                return false;
            }

            if (CustomersUsedBy.Any())
            {
                var customer =
                    CustomersUsedBy.Find(cust => (cust.CouponCode == cart.CouponCode && cust.CouponCode != null) ||
                                                 (cust.Email == cart.Customer.Email && cust.Email != null) ||
                                                 (cust.SocialSecurityNumber == cart.Customer.SocialSecurityNumber && cust.SocialSecurityNumber != null));
                return customer == null || customer.CouponUses < UseLimit;
            }

            return true;
        }

        /// <summary>
        ///     Returns the dicount in amount of money, this method may have side effects like adding a free product to the cart
        ///     and shuld therfore only evere be called once per coupon if it's actually valid.
        /// </summary>
        public abstract Decimal CalculateDiscount(Cart cart);

    }
}