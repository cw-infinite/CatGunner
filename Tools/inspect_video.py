import sys
sys.path.insert(0,'.tools')
import cv2
from PIL import Image, ImageDraw
from pathlib import Path
c=cv2.VideoCapture(r'C:\Users\deadm\Downloads\Idle Cat Gunner_ Shooter RPG (by Chodun) IOS Gameplay Video (HD)_720p.mp4')
mode=sys.argv[1]
times=[float(x) for x in sys.argv[2:]] if mode=='frames' else list(range(0,713,3))
if mode=='frames':
 for t in times:
  c.set(0 if False else cv2.CAP_PROP_POS_MSEC,t*1000);ok,f=c.read()
  if ok:cv2.imwrite(f'Analysis/frame_{t:07.2f}.jpg',f)
else:
 for page in range(0,len(times),24):
  sheet=Image.new('RGB',(8*147,3*344),'#20232b');d=ImageDraw.Draw(sheet)
  for i,t in enumerate(times[page:page+24]):
   c.set(cv2.CAP_PROP_POS_MSEC,t*1000);ok,f=c.read()
   if not ok:continue
   im=Image.fromarray(cv2.cvtColor(f,cv2.COLOR_BGR2RGB));im.thumbnail((147,320));x=i%8*147;y=i//8*344
   sheet.paste(im,(x,y+24));d.text((x+4,y+4),f'{t//60:02}:{t%60:02}',fill='white')
  sheet.save(f'Analysis/dense_{page//24:02}.jpg')
