using Xunit;
using Moq;
using System.Threading.Tasks;
using LibraryManagement.Controllers;
using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Services;
using AutoMapper;
using LibraryManagement.Entities;
using LibraryManagement.Dtos;

namespace LibraryManagement.Tests.Controllers 
{
    public class MembersControllerTests
    {
        private readonly Mock<IMemberService> _mockMemberService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly MembersController _membersController;

        public MembersControllerTests()
        {
            _mockMemberService = new Mock<IMemberService>();
            _mockMapper = new Mock<IMapper>();
            _membersController = new MembersController(_mockMemberService.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllMembers_ShouldReturnOkResult_WithMembers()
        {
            // Given
            var members = new List<Member> { new Member { Id = Guid.NewGuid(), Name = "John Doe", BorrowedBooksCount = 2 } };
            _mockMemberService.Setup(service => service.GetAllMembersAsync()).ReturnsAsync(members);

            var memberDtos = members.Select(member => new MemberDto { Id = member.Id, Name = member.Name, BorrowedBooksCount = member.BorrowedBooksCount });
            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<MemberDto>>(members)).Returns(memberDtos);

            // When
            var result = await _membersController.GetAllMembers();

            // Then
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnMembers = Assert.IsAssignableFrom<IEnumerable<MemberDto>>(okResult.Value);
            Assert.Single(returnMembers);
        }

        [Fact]
        public async Task GetMemberById_ShouldReturnOkResult_WithMember()
        {
            // Given
            var memberId = Guid.NewGuid();
            var member = new Member { Id = memberId, Name = "John Doe", BorrowedBooksCount = 0 };
            _mockMemberService.Setup(service => service.GetMemberByIdAsync(memberId)).ReturnsAsync(member);

            var memberDto = new MemberDto { Id = member.Id, Name = member.Name, BorrowedBooksCount = member.BorrowedBooksCount };
            _mockMapper.Setup(mapper => mapper.Map<MemberDto>(member)).Returns(memberDto);

            // When
            var result = await _membersController.GetMemberById(memberId);

            // Then
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnMember = Assert.IsType<MemberDto>(okResult.Value);
            Assert.Equal(memberId, returnMember.Id);
        }

        [Fact]
        public async Task GetMemberById_ShouldReturnNotFound_WhenMemberDoesNotExist()
        {
            // Given
            var nonExistentMemberId = Guid.NewGuid();
            _mockMemberService.Setup(service => service.GetMemberByIdAsync(nonExistentMemberId))
                .ReturnsAsync((Member)null);

            // When
            var result = await _membersController.GetMemberById(nonExistentMemberId);

            // Then
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task AddMember_ShouldReturnCreatedAtActionResult()
        {
            // Given
            var createMemberDto = new CreateMemberDto { Name = "John Doe" };
            var member = new Member { Id = Guid.NewGuid(), Name = createMemberDto.Name, BorrowedBooksCount = 0 };

            _mockMapper.Setup(mapper => mapper.Map<Member>(createMemberDto)).Returns(member);
            _mockMemberService.Setup(service => service.AddMemberAsync(member)).Returns(Task.CompletedTask);

            // When
            var result = await _membersController.AddMember(createMemberDto);

            // Then
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnMember = Assert.IsType<Member>(createdAtActionResult.Value);
            Assert.Equal(member.Name, returnMember.Name);
        }

        [Fact]
        public async Task UpdateMember_ShouldReturnNoContentResult_WhenUpdateIsSuccessful()
        {
            // Given
            var memberDto = new MemberDto { Id = Guid.NewGuid(), Name = "Updated Name", BorrowedBooksCount = 1 };
            var member = new Member { Id = memberDto.Id, Name = memberDto.Name, BorrowedBooksCount = memberDto.BorrowedBooksCount };

            _mockMapper.Setup(mapper => mapper.Map<Member>(memberDto)).Returns(member);
            _mockMemberService.Setup(service => service.UpdateMemberAsync(member)).Returns(Task.CompletedTask);

            // When
            var result = await _membersController.UpdateMember(memberDto.Id, memberDto);

            // Then
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateMember_ShouldReturnBadRequest_WhenIdsDoNotMatch()
        {
            // Given
            var memberId = Guid.NewGuid();
            var memberDto = new MemberDto { Id = Guid.NewGuid(), Name = "Name", BorrowedBooksCount = 0 };

            // When
            var result = await _membersController.UpdateMember(memberId, memberDto);

            // Then
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task UpdateMember_ShouldReturnNotFound_WhenMemberDoesNotExist()
        {
            // Given
            var memberDto = new MemberDto { Id = Guid.NewGuid(), Name = "Non-existent Member", BorrowedBooksCount = 0 };
            _mockMapper.Setup(mapper => mapper.Map<Member>(memberDto)).Returns(new Member { Id = memberDto.Id, Name = "Name" });
            _mockMemberService.Setup(service => service.UpdateMemberAsync(It.IsAny<Member>()))
                .Throws(new InvalidOperationException("Member not found"));

            // When
            var result = await _membersController.UpdateMember(memberDto.Id, memberDto);

            // Then
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteMember_ShouldReturnNoContentResult()
        {
            // Given
            var memberId = Guid.NewGuid();
            _mockMemberService.Setup(service => service.DeleteMemberAsync(memberId)).Returns(Task.CompletedTask);

            // When
            var result = await _membersController.DeleteMember(memberId);

            // Then
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteMember_ShouldReturnNotFound_WhenMemberDoesNotExist()
        {
            // Given
            var nonExistentMemberId = Guid.NewGuid();
            _mockMemberService.Setup(service => service.DeleteMemberAsync(nonExistentMemberId))
                .Throws(new InvalidOperationException("Member not found"));

            // When
            var result = await _membersController.DeleteMember(nonExistentMemberId);

            // Then
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}