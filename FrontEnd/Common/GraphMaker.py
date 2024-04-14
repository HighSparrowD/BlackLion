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


def nature_test_graph(x: int, y: int,
                      top_text: str,
                      bottom_text: str,
                      left_text: str,
                      right_text: str,
                      text_under_dot: str):
    # Setting backend renderer to avoid errors and make a better image
    matplotlib.use('agg')

    # Set style of the graph
    plt.style.use("Common/Styles/graph-dark-style.mplstyle")

    ax = plt.subplot()

    # Set length of axes
    ax.set_xlim(0, 24)
    ax.set_ylim(0, 24)

    # Draw ticks
    ax.set_xticks(range(0, 25))
    ax.set_yticks(range(0, 25))

    # Move the left and bottom spines to center, respectively.
    ax.spines[['left', 'bottom']].set_position('center')

    # Hide the top and right spines.
    ax.spines[["top", "right"]].set_visible(False)

    # Write labels of character
    plt.text(12, 25, top_text, ha='center')
    plt.text(12, -1.3, bottom_text, ha='center')
    plt.text(-1, 12 - len(left_text) / 2, '\n'.join(left_text[i] for i in range(len(left_text))), ha='right')
    plt.text(25, 12 - len(right_text) / 2, '\n'.join(right_text[i] for i in range(len(right_text))))

    # Text under dot
    plt.text(x, y-2, text_under_dot, fontweight='bold', ha='center')

    # Delete ticks on axes
    ax.tick_params(length=0)

    ax.plot(x, y, 'o', markersize=10)

    # Saving graph with base64
    stringIObytes = io.BytesIO()
    plt.savefig(stringIObytes, format='jpg', bbox_inches='tight')
    stringIObytes.seek(0)
    base64_encoded = base64.b64encode(stringIObytes.read())
    plt.clf()
    return base64.b64decode(base64_encoded)
