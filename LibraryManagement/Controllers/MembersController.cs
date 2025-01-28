using AutoMapper;
using LibraryManagement.Dtos;
using LibraryManagement.Entities;
using LibraryManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembersController : ControllerBase
    {
        private readonly IMemberService _memberService;
        private readonly IMapper _mapper;

        public MembersController(IMemberService memberService, IMapper mapper)
        {
            _memberService = memberService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetAllMembers()
        {
            var members = await _memberService.GetAllMembersAsync();
            var memberDtos = _mapper.Map<IEnumerable<MemberDto>>(members);
            return Ok(memberDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MemberDto>> GetMemberById(Guid id)
        {
            var member = await _memberService.GetMemberByIdAsync(id);
            if (member == null) return NotFound();
            var memberDto = _mapper.Map<MemberDto>(member);
            return Ok(memberDto);
        }

        [HttpPost]
        public async Task<ActionResult> AddMember([FromBody] CreateMemberDto createMemberDto)
        {
            var member = _mapper.Map<Member>(createMemberDto);
            await _memberService.AddMemberAsync(member);
            return CreatedAtAction(nameof(GetMemberById), new { id = member.Id }, member);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateMember(Guid id, [FromBody] MemberDto memberDto)
        {
            if (id != memberDto.Id) return BadRequest();
            try
            {
                var member = _mapper.Map<Member>(memberDto);
                await _memberService.UpdateMemberAsync(member);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMember(Guid id)
        {
            try 
            {
                await _memberService.DeleteMemberAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }
    }
}