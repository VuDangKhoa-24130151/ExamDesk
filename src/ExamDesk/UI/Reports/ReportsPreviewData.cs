namespace ExamDesk.UI.Reports;

// dữ liệu tạm cho UI
internal static class ReportsPreviewData
{
    public const string ManualSubmit = "Nộp bài";
    public const string AutoSubmit = "Hết giờ";
    public const string CA01 = "CA01 - Cơ sở dữ liệu - 07:00 06/11";
    public const string CA02 = "CA02 - Lập trình .NET - 09:35 06/11";
    public static List<string> Session() => new() { Ca01, Ca02 };
    public static List<string> Classes() => new() { "DH24IT01", "DH24IT02" };

    //Trả về danh sách mới
    public static List<AttemptResultItem> Attempts() => new() {
        new() { Mssv = "24130001", HoTen = "Nguyễn Văn An", Lop = "DH24IT01", Ca = Ca01, SoCau = 30, CauDung = 27, CauBoTrong = 0, CachNop = ManualSubmit, NopLuc = new DateTime(2026, 11, 6, 8, 5, 0) },
        new() { Mssv = "24130002", HoTen = "Trần Thị Bình",   Lop = "DH24IT01", Ca = Ca01, SoCau = 30, CauDung = 18, CauBoTrong = 3, CachNop = AutoSubmit,   NopLuc = new DateTime(2026, 11, 6, 8, 15, 0) },
        new() { Mssv = "24130003", HoTen = "Lê Minh Cường",   Lop = "DH24IT02", Ca = Ca01, SoCau = 30, CauDung = 12, CauBoTrong = 6, CachNop = AutoSubmit,   NopLuc = new DateTime(2026, 11, 6, 8, 15, 0) },
        new() { Mssv = "24130004", HoTen = "Phạm Thu Dung",   Lop = "DH24IT02", Ca = Ca01, SoCau = 30, CauDung = 22, CauBoTrong = 1, CachNop = ManualSubmit, NopLuc = new DateTime(2026, 11, 6, 8, 9, 0) },
        new() { Mssv = "24130005", HoTen = "Hoàng Văn Em",    Lop = "DH24IT01", Ca = Ca02, SoCau = 40, CauDung = 31, CauBoTrong = 2, CachNop = ManualSubmit, NopLuc = new DateTime(2026, 11, 6, 14, 20, 0) },
        new() { Mssv = "24130006", HoTen = "Võ Ngọc Giang",   Lop = "DH24IT02", Ca = Ca02, SoCau = 40, CauDung = 15, CauBoTrong = 10, CachNop = AutoSubmit,  NopLuc = new DateTime(2026, 11, 6, 14, 30, 0) },
    };

    // dữ liệu đúng sai,bỏ trống bài
     public static List<AnswerDetailItem> AnswerDetails(AttemptResultItem attempt){
        String[] options = { "A", "B", "C", "D" };

        // cố định theo MSSV (mở cùng 1 bài sẽ thấy giống bài đó)
        var random =new Random(attempt.Mssv.Sum(c => c));

        // 0 = đúng, 1 = bỏ trống, 2 = sai; xáo trộn để không dồn câu đúng lên đầu
        var kinds = Enumerable.Repeat(0, attempt.SoDung)
            .Concat(Enumerable.Repeat(1, attempt.SoBoTrong))
            .Concat(Enumerable.Repeat(2, attempt.SoSai))
            .OrderBy(_ => random.Next())
            .ToList();
        var details = new List<AnswerDetailItem>();
        for (int i = 0; i < kinds.Count; i++)
        {
            string correct = options[random.Next(4)];
            string selected = kinds[i] switch
            {
                0 => correct,
                1 => string.Empty,
                _ => options[(Array.IndexOf(options, correct) + 1 + random.Next(3)) % 4]
            };
            // Lịch sử đổi đáp án tạm: thỉnh thoảng giả lập SV đã đổi một lần
            string history = selected == string.Empty ? "(không chọn)" : selected;
            if (selected != string.Empty && random.Next(5) == 0)
            {
                string first = options[(Array.IndexOf(options, selected) + 1) % 4];
                history = $"{first} → {selected}";
            }
            details.Add(new AnswerDetailItem
            {
                Cau = i + 1,
                LuaChon = selected,
                DapAnDung = correct,
                LichSu = history
            });
        }
        return details;
    }

     }

// Dòng trong bảng kết quả
     internal sealed class AttemptResultItem
     {
        public string Mssv { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public string Lop { get; set; } = string.Empty;
        public string Ca { get; set; } = string.Empty;
        public int SoCau { get; set; }
        public int CauDung { get; set; };
        public int CauBoTrong { get; set; };
        public int CauSai => SoCau - CauDung - CauBoTrong;
        public string CachNop { get; set; } = string.Empty;
        public DateTime NopLuc { get; set; };

        //Điểm = số đúng / tổng x 10 , làm tròn 2 chữ số
        public double Diem => SoCau == 0 ? 0 : Math.Round(CauDung * 10.0 / SoCau, 2);
    
}
// Dòng chi tiết làm bài
     internal sealed class AnswerDetailItem
     {
        public int Cau { get; set; }
    public string LuaChon { get; set; } = string.Empty;   
    public string DapAnDung { get; set; } = string.Empty;
    public string LichSu { get; set; } = string.Empty;
    public string KetQua => LuaChon == string.Empty ? "Bỏ trống"
                          : LuaChon == DapAnDung ? "Đúng" : "Sai";
}

