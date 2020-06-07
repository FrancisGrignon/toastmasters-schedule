/*
SELECT * 
  FROM Members
 WHERE (Notify  = 1 AND Email  IS NOT NULL) 
    OR (Notify2 = 1 AND Email2 IS NOT NULL)
    OR (Notify3 = 3 AND Email3 IS NOT NULL)
    */
SELECT Email Email FROM Members WHERE (Notify  = 1 AND Email IS NOT NULL) 
UNION
SELECT Email2 Email FROM Members WHERE (Notify2  = 1 AND Email2 IS NOT NULL) 
UNION
SELECT Email3 Email FROM Members WHERE (Notify3  = 1 AND Email3 IS NOT NULL) 
ORDER BY Email