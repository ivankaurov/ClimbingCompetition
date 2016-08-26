
set nocount on
go

declare @rn int,
      @rTmp int

select @rn=routeNumber FROM lists(nolock) where iid = 9

declare @toExec varchar(5000),
         @rName varchar(5),
          @cInt varchar(3)

set @toExec = 
'   SELECT b.*'
set @rTmp = 0
while @rTmp < @rn begin
  set @rTmp = @rTmp + 1
  set @cInt = convert(varchar(3),@rTmp)
  set @rName = 'r' + @cInt + '.'
  set @toExec = @toExec + ', '+@rName+'topA t'+@cInt+', '+@rName+'bonusA b'+@cInt
end

set @toExec = @toExec+' FROM boulderResults b(NOLOCK) '
set @rTmp = 0
while @rTmp < @rn begin
  set @rTmp = @rTmp + 1
  set @rName = 'r' + convert(varchar(3),@rTmp)
  set @toExec = @toExec + ' LEFT JOIN BoulderRoutes '+@rName+'(NOLOCK) ON '+@rName+'.iid_parent = b.iid AND '+@rName+'.routeN = '+
                           CONVERT(VARCHAR(3),@rTmp)
end
EXEC(@toExec)
go
set nocount off

--select * from boulderroutes