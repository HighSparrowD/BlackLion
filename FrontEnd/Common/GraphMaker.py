import matplotlib
import matplotlib.pyplot as plt
import io
import base64


def graph_one_x(*args, x, xlabel=None, ylabel=None, legend_location: str = 'best'):
    """
    :param args: tuple of (y data, label, color)
    :param x: x data
    :param xlabel: label for x-axis
    :param ylabel: label for y-axis
    :param legend_location: location of legend. See https://matplotlib.org/stable/api/legend_api.html
    :return: b64-decoded graph photo in bytes
    """
    # Setting backend renderer to avoid errors and make a better image
    matplotlib.use('agg')

    # Set style of the graph
    plt.style.use("Common/Styles/graph-dark-style.mplstyle")

    for y, label, color in args:
        plt.plot(x, y, label=label, color=color)

    # Add legend
    plt.legend(loc=legend_location)

    # Axes labels
    plt.xlabel(xlabel)
    plt.ylabel(ylabel)

    # Saving graph with base64
    stringIObytes = io.BytesIO()
    plt.savefig(stringIObytes, format='jpg', bbox_inches='tight')
    stringIObytes.seek(0)
    base64_encoded = base64.b64encode(stringIObytes.read())
    plt.clf()
    return base64.b64decode(base64_encoded)
