using Microsoft.EntityFrameworkCore;
using Plan.Models;

namespace Plan.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<DisiplinMaster> DisiplinMaster { get; set; }
        public DbSet<KategoriEquipmentMaster> KategoriEquipmentMaster { get; set; }
        public DbSet<StatusKontrakMaster> StatusKontrakMaster { get; set; }

         public DbSet<AreaMaster> AreaMaster { get; set; }
         public DbSet<DireksiMaster> DireksiMaster { get; set; }
         public DbSet<KategoriMaintenanceMaster> KategoriMaintenanceMaster { get; set; }
         public DbSet<PlannerMaster> PlannerMaster { get; set; }
         public DbSet<StatusNextMaster> StatusNextMaster { get; set; }
         public DbSet<JenisKontrakMaster> JenisKontrakMaster { get; set; }
         public DbSet<FasePlanning> FasePlanning { get; set; }
         public DbSet<FaseTender> FaseTender { get; set; }

        public DbSet<FaseExecution> FaseExecution { get; set; }

        public DbSet<FaseMonitoring> FaseMonitoring { get; set; }
        public DbSet<Termin> Termin { get; set; }

        public DbSet<Timeline> Timeline { get; set; }

          public DbSet<Timelineok> Timelineok { get; set; }

            public DbSet<User> User { get; set; }
            public DbSet<PicStatusKontrakMaster> PicStatusKontrakMaster { get; set; }
            public DbSet<ResumeStatusKontrakMaster> ResumeStatusKontrakMaster { get; set; }
            public DbSet<KategoriKontrakMaster> KategoriKontrakMaster { get; set; }
            public DbSet<BiayaRealisasi> BiayaRealisasi { get; set; }
            public DbSet<LongTermServiceAgreement> LongTermServiceAgreement { get; set; }
            public DbSet<Budget> Budget { get; set; }

            public DbSet<OtorisasiKontrakMaster> OtorisasiKontrakMaster { get; set; }
            public DbSet<DAM> DAM { get; set; }
            public DbSet<History> History { get; set; }
            public DbSet<TaskMaster> TaskMaster { get; set; }
            public DbSet<TabelSla> TabelSla { get; set; }
            public DbSet<PersenTagihan> PersenTagihan { get; set; }
            public DbSet<BudgetWbsData> BudgetWbsData { get; set; }
            public DbSet<WbsData> WbsData { get; set; }
            public DbSet<Memo> Memo { get; set; }
            public DbSet<Tahapan> Tahapan { get; set; }
            public DbSet<Irkap> Irkap { get; set; }
            public DbSet<TabelDokumen> TabelDokumen { get; set; }
            public DbSet<HistoryTimelineok> HistoryTimelineok { get; set; }
            public DbSet<HistoryTermin> HistoryTermin { get; set; }
            public DbSet<HistoryFaseMonitoring> HistoryFaseMonitoring { get; set; }
            public DbSet<HistoryFaseExecution> HistoryFaseExecution { get; set; }

            public DbSet<HistoryFasePlanning> HistoryFasePlanning { get; set; }
            public DbSet<TabelComponent> TabelComponent { get; set; }

            public DbSet<TabelDokumenTimelineok> TabelDokumenTimelineok { get; set; }
            public DbSet<TabelDokumenTermin> TabelDokumenTermin { get; set; }




    }
}
