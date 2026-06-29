from PIL import Image

def green_to_transparency(input_file_path, output_file_path):
    img = Image.open(input_file_path)
    img = img.convert("RGBA")

    datas = img.getdata()

    newData = []
    for item in datas:
        if item[1] > 120 and item[0] < 120 and item[2] < 120:
            newData.append((0, 0, 0, 0))
        else:
            newData.append(item)

    img.putdata(newData)
    img.save(output_file_path, "PNG")

input_path = "brother_green.png"
output_path = "brother_transparent.png"
green_to_transparency(input_path, output_path)
