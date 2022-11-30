import matplotlib.pyplot as plt
from random import random

def intersect(p1, p2, p3, p4):
    x1,y1 = p1
    x2,y2 = p2
    x3,y3 = p3
    x4,y4 = p4
    denom = (y4-y3)*(x2-x1) - (x4-x3)*(y2-y1)
    if denom == 0: # parallel
        return None
    ua = ((x4-x3)*(y1-y3) - (y4-y3)*(x1-x3)) / denom
    print(ua)
    if ua < 0 or ua > 1: # out of range
        return None
    ub = ((x2-x1)*(y1-y3) - (y2-y1)*(x1-x3)) / denom
    if ub < 0 or ub > 1: # out of range
        return None
    x = x1 + ua * (x2-x1)
    y = y1 + ua * (y2-y1)
    return (int(x),int(y))

def plot_rects(p1, p2, p3, p4, label):  
    x1,y1 = p1
    x2,y2 = p2
    x3,y3 = p3
    x4,y4 = p4
   
    #plt.figure(figsize=(4,4))
    #plt.plot((x1,x2), (y1,y2), '.r--')
    #plt.plot((x3,x4), (y3,y4), '.b--')
    inter = intersect(p1, p2,p3,p4) 
    with open('text_debug_r.txt', "a+") as f:
       if inter is not None and inter != p1 and inter != p2 and inter != p3 and inter != p4:
          #qrcode1_l:qrcode2_l;qrcode2_l:qrcode3_l;0
          f.write(str(label) + "\n")

          f.write(str(p1) + ":" + str(p2) + ";" + str(p3) + ":" + str(p4) + ";" + str(inter) + "\n")
          #plt.plot(*inter, 'ok', markersize=10)

        #plt.savefig("intersection/intersection_" + str(label) + ".png")
    #plt.close()
    #plt.show()


with open('text_debug.txt', "r") as f:
    content = f.readlines()

for i in range(len(content)):
   
    #remover labels    
    if i % 2 == 0:
            
        #label = content[i].strip().split(";")
        #label = label[0].replace(":","_") + "___" + label[1].replace(":","_")
        label = content[i].strip().split(";")
        label = label[0] + ";" + label[1]
        continue      
    
    # reta 1
    pt1, pt2 = content[i].split(';')[0].split(':')
        
    pt1_x = int(pt1.split(',')[0])
    pt1_y = int(pt1.split(',')[1])
        
    pt2_x = int(pt2.split(',')[0])
    pt2_y = int(pt2.split(',')[1])
        
    pt1_reta1 = (pt1_x, pt1_y)
    pt2_reta1 = (pt2_x, pt2_y)

    # reta 2
    pt1, pt2 = content[i].split(';')[1].split(':')
        
    pt1_x = int(pt1.split(',')[0])
    pt1_y = int(pt1.split(',')[1])
        
    pt2_x = int(pt2.split(',')[0])
    pt2_y = int(pt2.split(',')[1])
        
    pt1_reta2 = (pt1_x, pt1_y)
    pt2_reta2 = (pt2_x, pt2_y)

    #print("reta 1: S-" + str(pt1_reta1) + " E-" + str(pt2_reta1))
    #print("reta 2: S-" + str(pt1_reta2) + " E-" + str(pt2_reta2))

    plot_rects(pt1_reta1, pt2_reta1, pt1_reta2, pt2_reta2, label)

