from PIL import Image

def green_to_transparency(input_file_path, output_file_path):
    img = Image.open(input_file_path)
    img = img.convert("RGBA")

    datas = img.getdata()

    newData = []
    for item in datas:
        if item[1] > 50 and item[0] < 200 and item[2] < 200:
            newData.append((0, 0, 0, 0))
        else:
            newData.append(item)

    img.putdata(newData)
    img.save(output_file_path, "PNG")

input_path = "zombie.png"
output_path = "zombie_transparent.png"
green_to_transparency(input_path, output_path)
