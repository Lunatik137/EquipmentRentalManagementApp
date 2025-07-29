using Xunit;
using Moq;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using DataAccess.Models;
using Repositories;
using Services;
using Repository;
using Service;

namespace EquipmentRentalManager.Tests
{
    public class RentalContractServiceTests
    {
        private readonly Mock<IRentalContractRepository> _repoMock;
        private readonly RentalContractService2 _service;

        public RentalContractServiceTests()
        {
            _repoMock = new Mock<IRentalContractRepository>();
            _service = new RentalContractService2(_repoMock.Object);
        }

        [Fact]
        public void Get_ShouldReturnContract_WhenIdExists()
        {
            var contract = new RentalContract { ContractId = 1 };
            _repoMock.Setup(r => r.Get(1)).Returns(contract);

            var result = _service.Get(1);

            result.Should().BeEquivalentTo(contract);
        }

        [Fact]
        public void Get_ShouldReturnNull_WhenIdNotExists()
        {
            _repoMock.Setup(r => r.Get(99)).Returns((RentalContract?)null);

            var result = _service.Get(99);

            result.Should().BeNull();
        }

        [Fact]
        public void GetByOwnerId_ShouldReturnEmptyList_WhenNoContracts()
        {
            _repoMock.Setup(r => r.GetByOwnerId(2)).Returns(new List<RentalContract>());

            var result = _service.GetByOwnerId(2);

            result.Should().BeEmpty();
        }

        [Fact]
        public void GetByOwnerId_ShouldReturnContracts_WhenContractsExist()
        {
            var contracts = new List<RentalContract>
            {
                new RentalContract { ContractId = 1, UserId = 1 },
                new RentalContract { ContractId = 2, UserId = 1 }
            };
            _repoMock.Setup(r => r.GetByOwnerId(1)).Returns(contracts);

            var result = _service.GetByOwnerId(1);

            result.Should().BeEquivalentTo(contracts);
        }

        [Fact]
        public void SearchByUserId_ShouldReturnMatchingContracts()
        {
            var contracts = new List<RentalContract>
            {
                new RentalContract { ContractId = 1, Status = "Đã thanh toán" },
                new RentalContract { ContractId = 2, Status = "Đã thanh toán" }
            };

            _repoMock.Setup(r => r.SearchByUserId(1, "Đã"))
                .Returns(contracts.Where(c => c.Status != null && c.Status.Contains("Đã")));

            var result = _service.SearchByUserId(1, "Đã");

            result.Should().HaveCount(2);
        }

        [Fact]
        public void SearchByUserId_ShouldReturnEmpty_WhenNoMatch()
        {
            var contracts = new List<RentalContract>
            {
                new RentalContract { ContractId = 1, Status = "Đã thanh toán" },
                new RentalContract { ContractId = 2, Status = "Đã thanh toán" }
            };

            _repoMock.Setup(r => r.SearchByUserId(1, "Đã"))
                .Returns(contracts.Where(c => c.Status != null && c.Status.Contains("Đã")));

            var result = _service.SearchByUserId(1, "Không");

            result.Should().BeEmpty();
        }

        [Fact]
        public void GetReturnedContractsByOwnerId_ShouldReturnOnlyReturnedContracts()
        {
            var contracts = new List<RentalContract>
            {
                new RentalContract { ContractId = 1, Status = "Đã trả" },
                new RentalContract { ContractId = 2, Status = "đã thanh toán" },
                new RentalContract { ContractId = 3, Status = "đã trả" }
            };

            _repoMock.Setup(r => r.GetByOwnerId(1)).Returns(contracts);

            var result = _service.GetReturnedContractsByOwnerId(1);

            result.Should().HaveCount(2);
            result.All(c => c.Status!.ToLower() == "đã trả").Should().BeTrue();
        }

        [Fact]
        public void GetReturnedContractsByOwnerId_ShouldReturnEmpty_WhenNoReturnedContracts()
        {
            var contracts = new List<RentalContract>
            {
                new RentalContract { ContractId = 1, Status = "Đang thuê" },
                new RentalContract { ContractId = 2, Status = null }
            };

            _repoMock.Setup(r => r.GetByOwnerId(3)).Returns(contracts);

            var result = _service.GetReturnedContractsByOwnerId(3);

            result.Should().BeEmpty();
        }

        [Fact]
        public void GetReturnedContractsByOwnerId_ShouldHandleEmptyList()
        {
            _repoMock.Setup(r => r.GetByOwnerId(10)).Returns(new List<RentalContract>());

            var result = _service.GetReturnedContractsByOwnerId(10);

            result.Should().BeEmpty();
        }

        [Fact]
        public void GetReturnedContractsByOwnerId_ShouldIgnoreCaseInStatus()
        {
            var contracts = new List<RentalContract>
            {
                new RentalContract { ContractId = 1, Status = "đã trả" },
                new RentalContract { ContractId = 2, Status = "ĐÃ TRẢ" },
                new RentalContract { ContractId = 3, Status = "Đang thuê" }
            };

            _repoMock.Setup(r => r.GetByOwnerId(4)).Returns(contracts);

            var result = _service.GetReturnedContractsByOwnerId(4);

            result.Should().HaveCount(2);
        }
    }
}
