import sys, json
sys.path.insert(0, '.tools')
import cv2
from PIL import Image, ImageDraw
from pathlib import Path
p=r'C:\Users\deadm\Downloads\Idle Cat Gunner_ Shooter RPG (by Chodun) IOS Gameplay Video (HD)_720p.mp4'
c=cv2.VideoCapture(p)
fps=c.get(cv2.CAP_PROP_FPS); n=c.get(cv2.CAP_PROP_FRAME_COUNT)
meta={'fps':fps,'frames':n,'duration':n/fps,'width':c.get(3),'height':c.get(4)}
print(json.dumps(meta)); Path('Analysis/video_metadata.json').write_text(json.dumps(meta,indent=2))
times=list(range(0,int(n/fps),15))
for page in range(0,len(times),12):
    ts=times[page:page+12]; sheet=Image.new('RGB',(6*196,2*454),'#12232b'); d=ImageDraw.Draw(sheet)
    for i,t in enumerate(ts):
        c.set(cv2.CAP_PROP_POS_MSEC,t*1000); ok,f=c.read()
        if not ok:continue
        im=Image.fromarray(cv2.cvtColor(f,cv2.COLOR_BGR2RGB)); im.thumbnail((196,426))
        x=(i%6)*196;y=(i//6)*454
        sheet.paste(im,(x,y+24));d.text((x+8,y+4),f'{t//60:02}:{t%60:02}',fill='white')
    sheet.save(f'Analysis/contact_{page//12:02}.jpg')


