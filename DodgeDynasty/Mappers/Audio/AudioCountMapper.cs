using System.Linq;
using DodgeDynasty.Models.Audio;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.Audio
{
	public class AudioCountMapper : MapperBase<DraftPickAudio>
	{
		protected override void DoUpdate(DraftPickAudio model)
		{
			if (!string.IsNullOrEmpty(model.apiCode))
			{
				var now = Utilities.GetEasternTime();
				var audioCount = HomeEntity.AudioCounts.FirstOrDefault(o => o.CallDate == now.Date && o.AudioApiCode == model.apiCode);
				if (audioCount != null)
				{
					audioCount.CallCount++;
					audioCount.LastUpdateTimestamp = now;
				}
				else
				{
					HomeEntity.AudioCounts.AddObject(new Entities.AudioCount
					{
						AudioApiCode = model.apiCode,
						CallDate = now.Date,
						CallCount = 1,
						AddTimestamp = now,
						LastUpdateTimestamp = now,
					});
                }
				HomeEntity.SaveChanges();
            }
		}
	}
}
