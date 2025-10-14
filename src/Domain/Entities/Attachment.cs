using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
namespace ProjectManagement.Domain.Entities;
public class Attachment:BaseAuditableEntity
{
    public string NameFile  { get; set; }= default!; // ชื่อเอกสารที่อัพโหลด
    public string PathFile  { get; set; }= default!; // url 
    public long FileSize { get; set; } = default!; // ขนาดไฟล์
    public string FileExtension { get; set; } = default!; //นามสกุลไฟล์
    public string BucketOriginalName { get; set; } = default!;
    public string BucketOriginalPath { get; set; } = default!;
}
