import sys
sys.path.insert(0,'.tools')
import cv2
from PIL import Image,ImageDraw
c=cv2.VideoCapture(r'C:\Users\deadm\Downloads\Idle Cat Gunner_ Shooter RPG (by Chodun) IOS Gameplay Video (HD)_720p.mp4')
for start,step in [(9,.2),(20,.2),(52,.4),(129,.4),(185,.4),(435,.4)]:
 sheet=Image.new('RGB',(4*294,4*310),'#21252c');d=ImageDraw.Draw(sheet)
 for i in range(16):
  t=start+i*step;c.set(cv2.CAP_PROP_POS_MSEC,t*1000);ok,f=c.read()
  if not ok:continue
  im=Image.fromarray(cv2.cvtColor(f[200:772],cv2.COLOR_BGR2RGB)).resize((294,286));x=i%4*294;y=i//4*310
  sheet.paste(im,(x,y+24));d.text((x+4,y+4),f'{t:.2f}s',fill='white')
 sheet.save(f'Analysis/timing_{start}.jpg')
