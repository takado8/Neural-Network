import cv2
import numpy as np
import math
import socket
import sys
import os
from scipy import ndimage
from os import listdir
from os.path import isfile, join


def getBestShift(img):
    cy, cx = ndimage.measurements.center_of_mass(img)

    rows, cols = img.shape
    shiftx = np.round(cols / 2.0 - cx).astype(int)
    shifty = np.round(rows / 2.0 - cy).astype(int)

    return shiftx, shifty


def shift(img, sx, sy):
    rows, cols = img.shape
    M = np.float32([[1, 0, sx], [0, 1, sy]])
    shifted = cv2.warpAffine(img, M, (cols, rows))
    return shifted


def normalizeImg(inputbytes):
    # read the image
    # print(no)
    #gray = cv2.imread('rawTemp.bmp', 0)
    input2 = np.frombuffer(inputbytes,dtype="byte")
    gray = cv2.imdecode(input2, 0)
    # rescale it
    gray = cv2.resize(255 - gray, (28, 28))
    # better black and white version
    (thresh, gray) = cv2.threshold(gray, 128, 255, cv2.THRESH_BINARY | cv2.THRESH_OTSU)

    while np.sum(gray[0]) == 0:
        gray = gray[1:]

    while np.sum(gray[:, 0]) == 0:
        gray = np.delete(gray, 0, 1)

    while np.sum(gray[-1]) == 0:
        gray = gray[:-1]

    while np.sum(gray[:, -1]) == 0:
        gray = np.delete(gray, -1, 1)

    rows, cols = gray.shape

    if rows > cols:
        factor = 20.0 / rows
        rows = 20
        cols = int(round(cols * factor))
        # first cols than rows
        gray = cv2.resize(gray, (cols, rows))
    else:
        factor = 20.0 / cols
        cols = 20
        rows = int(round(rows * factor))
        # first cols than rows
        gray = cv2.resize(gray, (cols, rows))

    colsPadding = (int(math.ceil((28 - cols) / 2.0)), int(math.floor((28 - cols) / 2.0)))
    rowsPadding = (int(math.ceil((28 - rows) / 2.0)), int(math.floor((28 - rows) / 2.0)))
    gray = np.lib.pad(gray, (rowsPadding, colsPadding), 'constant')

    shiftx, shifty = getBestShift(gray)
    shifted = shift(gray, shiftx, shifty)
    gray = shifted
    # save the processed images
    cv2.imwrite('readyTemp.bmp', gray)
    return True


def tcp_server():
    host = "localhost"
    port = 5367
    msg = ""
    mySocket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    mySocket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    mySocket.bind((host, port))
    # print(socket.getfqdn())
    # print(socket.gethostbyname(socket.getfqdn()))
    mySocket.listen(1)
    print('waiting for connection...')
    conn, addr = mySocket.accept()
    print("Connection from: " + str(addr))
    #while True:
    data = conn.recv(16384)

     #   if not data:
      #      break
    resp = b'0'
    print("received")
    if normalizeImg(data):
        resp = b'1'
    #print(data)
    conn.send(resp)
    print("sent back!")
    conn.close()


if __name__ == '__main__':
    while True:
        tcp_server()

