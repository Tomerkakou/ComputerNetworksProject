import datetime

import requests
from selenium import webdriver
from selenium.webdriver.common.by import By
from bs4 import BeautifulSoup
import time
import random
import pyodbc

def get_image_type(image_data):
    # Check the image file signature to determine the format
    if len(image_data) >= 2 and image_data[0] == 0xFF and image_data[1] == 0xD8:
        return "jpeg"
    elif len(image_data) >= 3 and image_data[0] == 0x89 and image_data[1] == 0x50 and image_data[2] == 0x4E:
        return "png"
    elif len(image_data) >= 4 and image_data[0] == 0x47 and image_data[1] == 0x49 and image_data[2] == 0x46 and image_data[3] == 0x38:
        return "gif"
    elif len(image_data) >= 2 and image_data[0] == 0x42 and image_data[1] == 0x4D:
        return "bmp"
    return "unknown"

path = r"C:\Program Files\Google\Chrome\Application\chrome.exe"

driver = webdriver.Chrome()


url = 'https://ksp.co.il/web/cat/218'

# Load the webpage
driver.get(url)
time.sleep(3)

button=driver.find_element(By.CLASS_NAME, "jss100").click()
element = driver.find_element(By.XPATH,"//*[text()='English']").click()

time.sleep(5)
cls = input("Enter your class: ")
page_source = driver.execute_script("return document.body.innerHTML;")

soup = BeautifulSoup(page_source, 'html.parser')



elements_with_class = soup.find_all('div', class_='jss283')

res=[]
# Iterate over the elements found and do something
for element in elements_with_class[:40]:
    try:
        img = element.find('img', alt=lambda x: x and x.startswith('image of product')).get('src')
        name = element.find('h3',class_='MuiTypography-root MuiTypography-subtitle1').find('a').text
        price= float(element.find('div',class_='jss333 null').text.replace('₪',''))
        print(img)
        print(name)
        print(price)
        response = requests.get(img)
        if response.status_code != 200:
            continue
        img_data = response.content
        img_type=get_image_type(img_data)
        print(img_type)
        dis=random.randint(1, 6)
        priceDiscount=None
        if dis==6:
            priceDiscount=round(price*round(random.uniform(0.5, 0.9), 1),2)
        stock=random.randint(0, 30)
        res.append((0,'temp',name,stock,stock,price,priceDiscount,img_data,img_type,datetime.datetime.now(),'Keyboards'))
    except:
        continue
# Close the WebDriver
driver.quit()





# Connection parameters
connection_string = "Driver={ODBC Driver 17 for SQL Server};Server=(localdb)\\MSSQLLocalDB;Database=ComputerNetworksProject;Integrated_Security=True;"

try:
    # Establish a connection
    conn = pyodbc.connect(connection_string)

    # Create a cursor
    cursor = conn.cursor()

    insert_stmt = 'INSERT INTO Products (ProductStatus,Name,Description,Stock,AvailableStock,Price,PriceDiscount,Img,ImgType,Created,CategoryName) VALUES (?,?,?,?,?,?,?,?,?,?,?)'
    # Execute a SQL query


    # Fetch and print results
    for row in res:
        try:
            cursor.execute(insert_stmt,row)
        except:
            continue


    # Close cursor and connection
    cursor.commit()
    cursor.close()
    conn.close()

except pyodbc.Error as e:
    print(f"Error connecting to SQL Server: {e}")
"""
products = [
    (4, 'Neon Sticky', 'Bright neon sticky notes for reminders'),
    (5, 'Bayonetta 3', 'Themed notes for game fans'),
    (6, 'Yellow Notes', 'Classic sticky notes in yellow'),
    (7, 'Deli Notes L', 'Large yellow sticky notes for more space'),
    (8, 'Deli Notes M', 'Medium-sized, handy sticky notes'),
    (9, 'Deli 12-Pack', 'Bulk pack of yellow stickies'),
    (10, '3M Mini Post', 'Small-sized post-its for quick notes'),
    (11, '3M Post-It L', 'Large post-its for ample writing'),
    (20, 'BOSS Highlt', 'Vibrant highlighters in a pack of 6'),
    (23, 'PILOT G-2 S2G', 'Assorted rollerball pens, pack of 4'),
    (24, 'STABILO Fine', 'Fineliner pens in a wallet of 10'),
    (25, 'BOSS Pastel', 'Pastel highlighters, wallet of 6'),
    (28, 'STABILO Pen68', 'Colour pens, 1mm tip, pack of 10'),
    (30, 'PILOT Grip Bl', 'Fine tip blue ballpoint, pack of 12'),
    (31, 'PILOT Neon', 'Neon rollerball pens, pack of 4'),
    (33, 'PILOT Grip Bk', 'Fine tip black ballpoint, pack of 12'),
    (36, 'PILOT Grip Md', 'Medium tip blue ballpoint, pack of 12'),
    (37, 'Casio FX-991ES', 'Advanced scientific calculator'),
    (38, 'Casio FX-82ES', 'User-friendly scientific calculator'),
    (39, 'Casio FX-991EX', 'High-performance scientific calc'),
    (40, 'Casio FC-200V', 'Financial calculator, 2nd edition'),
    (44, 'Casio FC-100V', 'Financial calculator, versatile'),
    (45, 'Casio FC100V', 'Compact financial calculator'),
    (46, 'Casio MS-8F', 'Mini desktop calculator'),
    (47, 'Casio HR-8RC', 'Printing calculator for receipts'),
    (49, 'Casio FX-82MS', 'Reliable scientific calculator'),
    (52, 'Casio D-120F', 'Desktop calculator, large display'),
    (53, 'Canon LS-123K', 'Metallic blue, 12-digit calculator'),
    (54, 'Canon TS-1200T', '12-digit calculator, dual power'),
    (56, 'Casio HL-820LV', 'Pocket calculator, vertical view'),
    (57, 'Casio SL-797TV', 'Travel-friendly calculator'),
    (58, 'Casio MJ-100D', 'Electronic calculator, check function'),
    (59, 'Casio WD-220MS', 'Water-resistant desktop calculator'),
    (60, 'Casio LC-403TV', 'Large display, travel calculator'),
    (61, 'Difuzed GoW5', 'Gears of War inspired backpack'),
    (62, 'Difuzed MaryP', 'Mary Poppins themed backpack'),
    (63, 'Pikachu Pack', 'Pokemon-themed backpack'),
    (64, 'Fila 3-Comp', 'Spacious backpack with 3 compartments'),
    (65, 'Marvel AOP', 'Avenger themed all-over print backpack'),
    (66, 'Gameboy Pack', 'Nintendo Gameboy print backpack'),
    (67, 'Atari 15L', 'Atari themed men’s backpack'),
    (68, 'Zelda Link', 'Zelda Toon Link green and black backpack'),
    (69, 'Pikachu BkPk', 'Pokemon Pikachu black backpack'),
    (70, 'Atari 15L', 'Atari themed men’s backpack'),
    (71, 'Zelda Link', 'Zelda Toon Link green and black backpack'),
    (72, 'Dr.Gav Jr 200', 'Junior gaming chair, black/blue'),
    (73, 'SL Looter', 'Gaming chair, black on black'),
    (74, 'Dragon Olym', 'Gaming chair, black/green'),
    (75, 'Scorpius Ch', 'Light grey fabric gaming chair'),
    (76, 'Dragon Combt', 'Gaming chair, black/red'),
    (77, 'Dragon Snipe', 'Gaming chair, white/pink'),
    (78, 'Dragon Snipe', 'Gaming chair, black/blue'),
    (79, 'Dr.Gav Jr Red', 'Junior gaming chair, black/red'),
    (80, 'Dr.Gav Jr Pink', 'Junior gaming chair, black/pink'),
    (81, 'Dragon Boss', 'Executive office chair, brown'),
    (82, 'Dragon Flex', 'Flexible office chair, black'),
    (83, 'Dragon MacTV', 'Gaming chair, blue/yellow'),
    (84, 'Dragon Olym P', 'Pink/white gaming chair for comfort'),
    (85, 'Logi G Pro X', 'Superlight wireless gaming mouse'),
    (86, 'Logi G502 X+', 'Wireless RGB gaming mouse, black'),
    (87, 'HX Pulsefire', 'Wireless white gaming mouse'),
    (89, 'Logi G703', 'Hero Lightspeed wireless mouse'),
    (90, 'Dragon RGB G9', 'Metallic RGB gaming mouse'),
    (91, 'Logi G102', 'Lightsync RGB gaming mouse, white'),
    (92, 'MX Master 3S', 'Wireless mouse, ergonomic design'),
    (93, 'Asus WT300', 'Wireless optical mouse, black'),
    (94, 'Logi M705', 'Marathon wireless mouse, retail'),
    (95, 'Apple Magic', 'White multi-touch surface mouse'),
    (96, 'Logi Lift Mac', 'Vertical ergonomic mouse, off-white'),
    (98, 'HX Haste Mini', 'Compact wireless gaming mouse'),
    (99, 'Roccat Kone', 'Ergonomic white gaming mouse'),
    (100, 'HX Pulsefire 2', 'Wireless gaming mouse, black'),
    (101, 'HX Pulsefire Mn', 'Compact wireless gaming mouse, black'),
    (102, 'HX Pulsefire Wh', 'Wireless gaming mouse, white'),
    (103, 'Logi G705 RBG', 'Wireless RBG gaming mouse, white'),
    (104, 'Roccat Burst', 'Air wireless RGB mouse, white'),
    (105, 'Roccat Kone Wht', 'Sleek white gaming mouse'),
    (106, 'Roccat Kone Blk', 'Sleek black gaming mouse'),
    (107, 'Logi G413 TKL', 'TKL mechanical gaming keyboard'),
    (108, 'SKYLOONG GK-61', 'White mechanical gaming keyboard'),
    (109, 'Dragon RGB KB', 'RGB gaming keyboard, black'),
    (110, 'SteelSeris Apx3', 'RGB gaming keyboard'),
    (111, 'SteelSeris Apx9', 'TKL gaming keyboard'),
    (112, 'Lenovo KM3', 'Gaming combo keyboard and mouse'),
    (113, 'Logi MK345', 'Wireless keyboard and mouse set'),
    (114, 'Apple Magic Heb', 'Magic keyboard with Hebrew layout'),
    (115, 'Logi Combo', 'Wireless keyboard and mouse set'),
    (116, 'Logi MK540 Adv', 'Advanced wireless keyboard and mouse'),
    (122, 'ASUS CW100 Set', 'Wireless keyboard and mouse set, black'),
    (123, 'SKYLOONG B', 'Bluetooth mechanical keyboard, black'),
    (124, 'SKYLOONG W', 'Bluetooth mechanical keyboard, white'),
    (128, 'Roccat Pyro', 'Mechanical keyboard with red switch')
]

# Connection parameters
connection_string = "Driver={ODBC Driver 17 for SQL Server};Server=(localdb)\\MSSQLLocalDB;Database=ComputerNetworksProject;Integrated_Security=True;"

try:
    # Establish a connection
    conn = pyodbc.connect(connection_string)

    # Create a cursor
    cursor = conn.cursor()


    # Fetch and print results
    for id,name,decription in products :
       update=f"update Products set name='{name}',description='{decription}' where id={id}"
       cursor.execute(update)

    cursor.commit()
    cursor.close()
    conn.close()

except pyodbc.Error as e:
    print(f"Error connecting to SQL Server: {e}")
"""    
