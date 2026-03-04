import sys
import json
from dataclasses import dataclass
import geopandas as gpd
import matplotlib.pyplot as plt
from shapely.geometry import Polygon, MultiPolygon, Point
import numpy as np

@dataclass
class PointWithState:
latitude: float
longitude: float
sentiment: float
state: str
color: tuple

def get_color(sentiment):
if sentiment == 0:
return (1.0, 1.0, 1.0)

code
Code
download
content_copy
expand_less
max_val = 2.0
ratio = min(abs(sentiment) / max_val, 1.0)

if sentiment > 0:
    return (1.0 - ratio, 1.0 - ratio, 1.0)
else:
    return (1.0, 1.0 - ratio, 1.0 - ratio)

raw_input = sys.stdin.read()

if not raw_input.strip():
sys.exit(1)

try:
points_data = json.loads(raw_input)
except json.JSONDecodeError:
sys.exit(1)

try:
with open("states.json", "r", encoding="utf-8") as f:
raw_states = json.load(f)
except FileNotFoundError:
sys.exit(1)

state_names = []
state_geometries = []

for state_name, polygons in raw_states.items():
shapely_polygons = []
for polygon in polygons:
coords = polygon
while isinstance(coords[0][0], list):
coords = coords[0]
shapely_polygons.append(Polygon(coords))

code
Code
download
content_copy
expand_less
if len(shapely_polygons) == 1:
    geometry = shapely_polygons[0]
else:
    geometry = MultiPolygon(shapely_polygons)

state_names.append(state_name)
state_geometries.append(geometry)

states = gpd.GeoDataFrame(
{"state": state_names},
geometry=state_geometries,
crs="EPSG:4326"
)

if not points_data:
sys.exit(0)

geometry = [Point(p["Longitude"], p["Latitude"]) for p in points_data]
points_gdf = gpd.GeoDataFrame(points_data, geometry=geometry, crs="EPSG:4326")

points_joined = gpd.sjoin(points_gdf, states, how="left", predicate="within")

points_with_state = []
state_sentiments = {}

for _, row in points_joined.iterrows():
sentiment = row["Sentiment"]
state_name = row["state"] if "state" in row and isinstance(row["state"], str) else "Unknown"

code
Code
download
content_copy
expand_less
color = get_color(sentiment)

points_with_state.append(PointWithState(
    latitude=row["Latitude"],
    longitude=row["Longitude"],
    sentiment=sentiment,
    state=state_name,
    color=color
))

if state_name != "Unknown":
    if state_name not in state_sentiments:
        state_sentiments[state_name] = []
    state_sentiments[state_name].append(sentiment)

state_average_color = {}

for state, sentiments in state_sentiments.items():
avg_sentiment = np.mean(sentiments)
state_average_color[state] = get_color(avg_sentiment)

def get_state_color(row):
st = row["state"]
if st in state_average_color:
return state_average_color[st]
return (0.8, 0.8, 0.8)

states["plot_color"] = states.apply(get_state_color, axis=1)

fig, ax = plt.subplots(figsize=(16, 10))

states.plot(
ax=ax,
color=states["plot_color"],
edgecolor="black",
linewidth=0.5
)

for point in points_with_state:
ax.scatter(
point.longitude,
point.latitude,
color=point.color,
s=30,
edgecolors='black',
linewidth=0.3,
zorder=5,
alpha=0.7
)

ax.set_xlim(-128, -65)
ax.set_ylim(24, 50)
ax.axis("off")

plt.title("USA Sentiment Map", fontsize=16)
plt.tight_layout()

manager = plt.get_current_fig_manager()
try:
manager.window.state('zoomed')
except Exception:
try:
manager.full_screen_toggle()
except Exception:
pass

plt.savefig("result.png", dpi=300, bbox_inches='tight')
plt.show()
