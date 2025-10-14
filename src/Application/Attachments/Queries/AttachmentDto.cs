using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.ActivityPlans.Queries;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Attachments.Queries;
public class AttachmentDto
{
    public string NameFile { get; set; } = default!; // ชื่อเอกสารที่อัพโหลด
    public string PathFile { get; set; } = default!; // url 
    public long FileSize { get; set; } = default!; // ขนาดไฟล์
    public string FileExtension { get; set; } = default!; //นามสกุลไฟล์
    public string BucketOriginalName { get; set; } = default!;
    public string BucketOriginalPath { get; set; } = default!;
    public string? ThumbnailBase64 { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Attachment, AttachmentDto>();
        }
    }

}
