import sys
import os
import ntpath
from subprocess import call
import tensorflow as tf
from tensorflow.python.platform import gfile

path = os.path.splitext(ntpath.split(sys.argv[1])[1])[0]
print(path)
with tf.Session() as sess:
    model_filename = sys.argv[1]
    with gfile.FastGFile(model_filename, 'rb') as f:
        graph_def = tf.GraphDef()
        graph_def.ParseFromString(f.read())
        g_in = tf.import_graph_def(graph_def)

if not os.path.exists(path):
    os.makedirs(path)

train_writer = tf.summary.FileWriter(path)
train_writer.add_graph(sess.graph)

call(["tensorboard", "--logdir="+path])