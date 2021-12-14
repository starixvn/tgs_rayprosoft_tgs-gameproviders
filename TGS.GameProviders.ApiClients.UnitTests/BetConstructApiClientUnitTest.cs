using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TGS.GameProviders.ApiClients.BetConstruct;

namespace TGS.GameProviders.ApiClients.UnitTests
{
    [TestFixture]
    public class BetConstructApiClientUnitTest
    {
        [Test]
        public void Test_BetPlaced_Hash_String()
        {
            var authToken = "your_client_security_token";
            var ts = 1461670530;
            var transactionId = 34234324;
            var betId = 123456;
            var amount = 123.00m;
            var bonusBetAmount = 10.00m;
            var created = "2016-04-26T11:35:30.054Z";
            var betType = 1;
            var systemMinCount = new object();
            var totalPrice = 2.500m;

            var dict = new Dictionary<string, object>
            {
                { "AuthToken", authToken},
                { "TS", ts },
                { "TransactionId", transactionId },
                { "BetId",betId },
                { "Amount", amount },
                { "Created", created},
                { "BetType", betType},
                { "TotalPrice", totalPrice}
            };

            var sharedKey = "your_shared_key";

            var content = string.Join(
                "",
                dict.Where(x => x.Value != null && x.Value.ToString() != "")
                    .Select(x =>
                    {
                        return $"{x.Key}{x.Value}";
                    }));

            var plainText = $"{content}{sharedKey}";
            var expected = "AuthTokenyour_client_security_tokenTS1461670530TransactionId34234324BetId123456Amount123.00Created2016-04-26T11:35:30.054ZBetType1TotalPrice2.500your_shared_key";

            var actual = string.Join("", dict.Select(x => $"{x.Key}{x.Value}")) + sharedKey;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_BetPlaced_Hash_String_With_Diff_Shared_Key()
        {
            var authToken = "your_client_security_token";
            var ts = 1461670530;
            var transactionId = 34234324;
            var betId = 123456;
            var amount = 123.00m;
            var bonusBetAmount = 10.00m;
            var created = "2016-04-26T11:35:30.0543787Z";
            var betType = 1;
            var systemMinCount = new object();
            var totalPrice = 2.500m;

            var dict = new Dictionary<string, object>
            {
                { "AuthToken", authToken},
                { "TS", ts },
                { "TransactionId", transactionId },
                { "BetId",betId },
                { "Amount", amount },
                { "Created", created},
                { "BetType", betType},
                { "TotalPrice", totalPrice}
            };

            var sharedKey = "jiLEPYw5n3n5kcx6T3";

            var content = string.Join(
                "",
                dict.Where(x => x.Value != null && x.Value.ToString() != "")
                    .Select(x =>
                    {
                        return $"{x.Key}{x.Value}";
                    }));

            var plainText = $"{content}{sharedKey}";
            var expected = "c636961a68cfddf9ab46d8661f3a2626";

            var utility = new BetConstructUtility();

            var hash = utility.MD5Hash(plainText);

            Assert.AreEqual(expected, hash);
        }

        [Test]
        public void Test_GetClientBalance_Hash_String()
        {
            var authToken = "your_client_security_token";
            var ts = 1461682696;

            var dict = new Dictionary<string, object>
            {
                { "AuthToken", authToken},
                { "TS", ts }
            };

            var sharedKey = "your_shared_key";

            var content = string.Join(
                "",
                dict.Where(x => x.Value != null && x.Value.ToString() != "")
                    .Select(x =>
                    {
                        return $"{x.Key}{x.Value}";
                    }));
            var expected = "AuthTokenyour_client_security_tokenTS1461682696your_shared_key";

            var actual = string.Join("", dict.Select(x => $"{x.Key}{x.Value}").ToArray()) + sharedKey;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_GetClientDetails()
        {
            var authToken = "ZYE5oSH5I1hzmrgnQnh3R6";
            var ts = 1461682696;
            var dict = new Dictionary<string, object>
            {
                { "AuthToken", authToken},
                { "TS", ts }
            };

            var sharedKey = "jiLEPYw5n3n5kcx6T3";

            var content = string.Join(
                "",
                dict.Where(x => x.Value != null && x.Value.ToString() != "")
                    .Select(x =>
                    {
                        return $"{x.Key}{x.Value}";
                    }));

            var plainText = $"{content}{sharedKey}";

            var utility = new BetConstructUtility();

            var hash = utility.MD5Hash(plainText);
        }

        [Test]
        public void Test_GetClientDetails_Hash_String()
        {
            var authToken = "your_client_security_token";
            var ts = 1461682696;

            var dict = new Dictionary<string, object>
            {
                { "AuthToken", authToken},
                { "TS", ts }
            };

            var sharedKey = "your_shared_key";

            var content = string.Join(
                "",
                dict.Where(x => x.Value != null && x.Value.ToString() != "")
                    .Select(x =>
                    {
                        return $"{x.Key}{x.Value}";
                    }));
            var expected = "AuthTokenyour_client_security_tokenTS1461682696your_shared_key";

            var actual = string.Join("", dict.Select(x => $"{x.Key}{x.Value}").ToArray()) + sharedKey;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestBetPlacedHash()
        {
            var authToken = "ZYE5oSH5I1hzmrgnQnh3R6";
            var ts = 1637897145;
            var transactionId = 49452053363;
            var betId = 1826054161;
            var amount = 1.00m;
            var created = new DateTimeOffset(new DateTime(2021, 11, 26, 3, 25, 45, 387));
            var betType = 1;
            var systemMinCount = "";
            var totalPrice = 3.570m;
            var hashDict = new Dictionary<string, object>
            {
                { "AuthToken" , authToken },
                { "TS", ts },
                { "TransactionId", transactionId},
                { "BetId", betId},
                { "Amount", amount },
                { "Created", created.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")},
                { "BetType", betType },
                { "SystemMinCount", systemMinCount},
                { "TotalPrice", totalPrice}
            };

            var sharedKey = "jiLEPYw5n3n5kcx6T3";

            var content = string.Join(
                "",
                hashDict.Where(x => x.Value != null && x.Value.ToString() != "")
                    .Select(x =>
                    {
                        return $"{x.Key}{x.Value}";
                    }));

            var plainText = $"{content}{sharedKey}";
            var expected = "0b2473b0a0d3f09b1e72ede256616039";

            var utility = new BetConstructUtility();

            var actual = utility.MD5Hash(plainText);

            Assert.AreEqual(expected, actual);
        }
    }
}
