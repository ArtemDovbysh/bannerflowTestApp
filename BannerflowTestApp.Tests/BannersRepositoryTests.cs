using System;
using System.Threading.Tasks;
using BannerflowTestApp.Data.Models;
using BannerflowTestApp.Data.Services;
using MongoDB.Driver;
using Xunit;

namespace BannerflowTestApp.Tests
{
    public class BannersRepositoryTests : IClassFixture<DocumentDbFixture>, IDisposable
    {
        private readonly DocumentDbFixture _fixture;

        public BannersRepositoryTests(DocumentDbFixture fixture)
        {
            _fixture = fixture;
            _fixture.CreateCollectionAsync().Wait();
        }

        public void Dispose()
        {
            _fixture.DeleteCollectionAsync().Wait();
        }

        [Fact]
        public async Task GetAllAsync_NoBanners_ReturnsEmptyCollection()
        {
            // Arrange
            var bannersRepository = new BannersRepository(_fixture.Options);

            // Act
            var result = await bannersRepository.GetAllAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAsync_NonExistingBannerId_ReturnsNull()
        {
            // Arrange
            var bannersRepository = new BannersRepository(_fixture.Options);

            // Act
            var result = await bannersRepository.GetAsync(123);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAsync_ExistingBannerId_ReturnsBanner()
        {
            // Arrange
            var newBanner = new Banner()
            {
                Id = 1,
                Html = "<html></html>",
                Created = new DateTime()
            };
            var bannersRepository = new BannersRepository(_fixture.Options);

            // Act
            await bannersRepository.CreateAsync(newBanner);
            var result = await bannersRepository.GetAsync(newBanner.Id);

            // Assert
            Assert.Equal(newBanner.Id, result.Id);
            Assert.Equal(newBanner.Html, result.Html);
        }

        [Fact]
        public async Task CreateAsync_SomeBanners_CreatedTimeIsIndependentToGiven()
        {
            // Arrange
            var bannersRepository = new BannersRepository(_fixture.Options);
            var startTime = DateTime.UtcNow;
            var newBanner = new Banner()
            {
                Id = 1,
                Html = "<html></html>",
                Created = DateTime.UtcNow.AddDays(-10)
            };

            // Act
            var result = await bannersRepository.CreateAsync(newBanner);
            await Task.Delay(0);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Created >= startTime);
            Assert.True(DateTime.UtcNow > result.Created);
        }

        [Fact]
        public async Task CreateAsync_SomeBanners_NewBannerModifiedTimeIsEmpty()
        {
            // Arrange
            var bannersRepository = new BannersRepository(_fixture.Options);
            var newBanner = new Banner()
            {
                Id = 1,
                Html = "<html></html>",
            };

            // Act
            var result = await bannersRepository.CreateAsync(newBanner);

            // Assert
            Assert.Null(result.Modified);
        }

        [Fact]
        public async Task GetAllAsync_SomeBanners_ReturnsNonEmptyCollection()
        {
            // Arrange
            var bannerId = GetRandomIntId();
            var newBanner = new Banner()
            {
                Id = bannerId,
                Html = "<html></html>",
            };
            var bannersRepository = new BannersRepository(_fixture.Options);

            // Act
            await bannersRepository.CreateAsync(newBanner);
            var allBanners = await bannersRepository.GetAllAsync();

            // Assert
            Assert.Single(allBanners, x => x.Id == bannerId);
        }

        [Fact]
        public async Task CreateAsync_Always_ImpossibleToInsertSeveralBroductsWithSameId()
        {
            // Arrange
            var bannerId = GetRandomIntId();
            var bannersRepository = new BannersRepository(_fixture.Options);

            // Act
            var result = await bannersRepository.CreateAsync(new Banner { Id = bannerId, Html = string.Empty});
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(bannerId, result.Id);
            await Assert.ThrowsAsync<MongoWriteException>(() => bannersRepository.CreateAsync(new Banner { Id = bannerId, Html = string.Empty }));
        }

        [Fact]
        public async Task CreateAsync_IncorrectHtml_ReturningException()
        {
            // Arrange
            var bannerId = GetRandomIntId();
            var bannersRepository = new BannersRepository(_fixture.Options);

            // Act

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(() => bannersRepository.CreateAsync(new Banner { Id = bannerId, Html = "<html></span>" }));
            await Assert.ThrowsAsync<ArgumentException>(() => bannersRepository.CreateAsync(new Banner { Id = bannerId + 1, Html = "<html></span></html>" }));
            await Assert.ThrowsAsync<ArgumentException>(() => bannersRepository.CreateAsync(new Banner { Id = bannerId + 2, Html = "<html></span><span></html>" }));
        }
        
        [Fact]
        public async Task UpdateAsync_NonExistingBanner_ReturnsNull()
        {
            // Arrange
            var bannerId = GetRandomIntId();
            var bannersRepository = new BannersRepository(_fixture.Options);
            var newBanner = new Banner()
            {
                Id = bannerId,
                Html = "<html></html>",
            };
            
            // Act
            await bannersRepository.UpdateAsync(newBanner);
            var banner = await bannersRepository.GetAsync(bannerId);

            // Assert
            Assert.Null(banner);
        }

        [Fact]
        public async Task UpdateAsync_ExistingBanner_ReturnsTrue()
        {
            // Arrange
            var bannerId = GetRandomIntId();
            const string expectedHtml = "<html></html>";
            var bannersRepository = new BannersRepository(_fixture.Options);

            // Act
            var existingBanner = await bannersRepository.CreateAsync(new Banner{Id = bannerId, Html = string.Empty });
            await bannersRepository.UpdateAsync(new Banner {Id = bannerId, Html = expectedHtml});
            var changedBanner = await bannersRepository.GetAsync(bannerId);

            // Assert
            Assert.NotNull(changedBanner);
            Assert.Equal(existingBanner.Id, changedBanner.Id);
            Assert.NotEqual(existingBanner.Html, changedBanner.Html);
            Assert.Equal(changedBanner.Html, expectedHtml);
        }

        [Fact]
        public async Task UpdateAsync_Always_SetsModified()
        {
            // Arrange
            var bannerId = GetRandomIntId();
            var bannersRepository = new BannersRepository(_fixture.Options);
            var newBanner = new Banner()
            {
                Id = bannerId,
                Html = "<html></html>",
            };

            // Act
            var createdBanner = await bannersRepository.CreateAsync(newBanner);
            await bannersRepository.UpdateAsync(new Banner { Id = bannerId, Html = "<html></html>" });
            var changedBanner = await bannersRepository.GetAsync(bannerId);

            // Assert
            Assert.Null(createdBanner.Modified);
            Assert.NotNull(changedBanner.Modified);
        }

        [Fact]
        public async Task UpdateAsync_Never_UpdatesCreated()
        {
            // Arrange
            var bannerId = GetRandomIntId();
            var bannersRepository = new BannersRepository(_fixture.Options);
            var newBanner = new Banner()
            {
                Id = bannerId,
                Html = "<html></html>",
            };

            // Act
            await bannersRepository.CreateAsync(newBanner);
            var createdBanner = await bannersRepository.GetAsync(bannerId);
            await bannersRepository.UpdateAsync(new Banner { Id = bannerId, Html = "<html></html>" });
            var changedBanner = await bannersRepository.GetAsync(bannerId);

            // Assert
            Assert.Equal(createdBanner.Created, changedBanner.Created);
        }
        
        [Fact]
        public async Task UpdateAsync_IncorrectHtml_ReturningException()
        {
            // Arrange
            var bannerId = GetRandomIntId();
            var bannersRepository = new BannersRepository(_fixture.Options);

            // Act
            await bannersRepository.CreateAsync(new Banner {Id = bannerId, Html = "<html></html>"});

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(() => bannersRepository.UpdateAsync(new Banner { Id = bannerId, Html = "<html></span>" }));
            await Assert.ThrowsAsync<ArgumentException>(() => bannersRepository.UpdateAsync(new Banner { Id = bannerId + 1, Html = "<html></span></html>" }));
            await Assert.ThrowsAsync<ArgumentException>(() => bannersRepository.UpdateAsync(new Banner { Id = bannerId + 2, Html = "<html></span><span></html>" }));
        }

        [Fact]
        public async Task DeleteAsync_NonExistingBanner_NoErrors()
        {
            // Arrange
            var bannerId = GetRandomIntId();
            var bannersRepository = new BannersRepository(_fixture.Options);

            // Act
            await bannersRepository.DeleteAsync(bannerId);

            // Assert
        }

        [Fact]
        public async Task DeleteAsync_ExistingBanner_ReturnsTrue()
        {
            // Arrange
            var bannerId = GetRandomIntId();
            var bannersRepository = new BannersRepository(_fixture.Options);

            // Act
            await bannersRepository.CreateAsync(new Banner {Id = bannerId, Html = string.Empty});
            await bannersRepository.DeleteAsync(bannerId);
            var result = await bannersRepository.GetAsync(bannerId);

            // Assert
            Assert.Null(result);
        }

        private static int GetRandomIntId()
        {
            Random rnd = new Random();
            return rnd.Next(1, 2048);
        }
    }
}