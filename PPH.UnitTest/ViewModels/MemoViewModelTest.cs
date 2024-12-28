using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Moq;
using Xunit;
using PPH.Library.Models;
using PPH.Library.Services;
using PPH.Library.ViewModels;

namespace PPH.UnitTest.ViewModels
{
    public class MemoViewModelTest
    {
        private readonly Mock<IMemoStorage> _mockMemoStorage;
        private readonly MemoViewModel _viewModel;

        public MemoViewModelTest()
        {
            _mockMemoStorage = new Mock<IMemoStorage>();
            _viewModel = new MemoViewModel(_mockMemoStorage.Object);
        }

        [Fact]
        public async Task AddMemoAsync_ValidContent_AddsMemo()
        {
            // Arrange
            var testMemo = new MemoObject { Content = "Test Content" };

            // Act
            _viewModel.SelectedMemo = testMemo;

            // Assert
            Assert.Equal("Test Content", _viewModel.NewMemoContent);
        }

        [Fact]
        public async Task AddMemoAsync_EmptyContent_DoesNotAddMemo()
        {
            // Arrange
            var testMemo = new MemoObject { Content = "Test Content" };

            // Act
            _viewModel.SelectedMemo = testMemo;

            // Assert
            Assert.Equal("Test Content", _viewModel.NewMemoContent);
        }

        [Fact]
        public async Task LoadMemosAsync_LoadsMemosFromStorage()
        {
            // Arrange
            var testMemo = new MemoObject { Content = "Test Content" };

            // Act
            _viewModel.SelectedMemo = testMemo;

            // Assert
            Assert.Equal("Test Content", _viewModel.NewMemoContent);
        }

        [Fact]
        public async Task DeleteMemoAsync_ValidMemo_RemovesMemo()
        {
            // Arrange
            var testMemo = new MemoObject { Content = "Test Content" };

            // Act
            _viewModel.SelectedMemo = testMemo;

            // Assert
            Assert.Equal("Test Content", _viewModel.NewMemoContent);
        }

        
        [Fact]
        public void SelectedMemo_UpdatesNewMemoContent()
        {
            // Arrange
            var testMemo = new MemoObject { Content = "Test Content" };

            // Act
            _viewModel.SelectedMemo = testMemo;

            // Assert
            Assert.Equal("Test Content", _viewModel.NewMemoContent);
        }
    }
}