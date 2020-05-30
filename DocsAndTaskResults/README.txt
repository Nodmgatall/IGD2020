we wrote out own world generation with chunks and noise layering
so the image is not what you probably expect.

We used a hight noise for 3 "bioms":
    - sand around the lowest parts of the noise, lower than sand is
      only water which is our default tile that is produced whenever we
      get a value for which we did not define any tiles;
    - grass
    - mountains.

inside the gras biom we use 2 other noise maps to decide where forests are and wher rock "resources" spawn.

