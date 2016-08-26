declare @iid bigint,@iid_new bigint, @n int,
@t1 int,@t2 int, @t3 int, @t4 int, @t5 int, @b1 int, @b2 int, @b3 int, @b4 int, @b5 int
declare crs cursor for select iid, top1, bonus1, top2, bonus2, top3, bonus3, top4,bonus4, top5, bonus5
from boulderresults(nolock)
open crs
fetch next from crs into @iid, @t1,@b1,@t2,@b2,@t3,@b3,@t4,@b4,@t5,@b5
while @@fetch_status = 0
begin
  select @iid_new = (isnull(max(iid),0) + 1) from boulderroutes(nolock)
  insert into boulderroutes(iid, iid_parent,routeN,topA, bonusA)
      values(@iid_new,@iid,1,@t1,@b1)
  
  select @iid_new = (max(iid) + 1) from boulderroutes(nolock)
  insert into boulderroutes(iid, iid_parent,routeN,topA, bonusA)
      values(@iid_new,@iid,2,@t2,@b2)
      
  select @iid_new = (max(iid) + 1) from boulderroutes(nolock)
  insert into boulderroutes(iid, iid_parent,routeN,topA, bonusA)
      values(@iid_new,@iid,3,@t3,@b3)
  
  select @iid_new = (max(iid) + 1) from boulderroutes(nolock)
  insert into boulderroutes(iid, iid_parent,routeN,topA, bonusA)
      values(@iid_new,@iid,4,@t4,@b4)
      
  select @iid_new = (max(iid) + 1) from boulderroutes(nolock)
  insert into boulderroutes(iid, iid_parent,routeN,topA, bonusA)
      values(@iid_new,@iid,5,@t5,@b5)
    
  fetch next from crs into @iid, @t1,@b1,@t2,@b2,@t3,@b3,@t4,@b4,@t5,@b5
end
close crs
deallocate crs
GO
select * from boulderroutes