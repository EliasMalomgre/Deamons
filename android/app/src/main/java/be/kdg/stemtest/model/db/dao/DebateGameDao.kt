package be.kdg.stemtest.model.db.dao

import androidx.room.*
import be.kdg.stemtest.model.entity.DebateGameResult
import io.reactivex.Maybe
import io.reactivex.Single

@Dao
abstract class DebateGameDao{

    @Insert(onConflict = OnConflictStrategy.REPLACE)
   abstract fun insert(debateGameResult: DebateGameResult)


    @Query("SELECT * FROM debategameresult")
  abstract  fun getResult(): Maybe<List<DebateGameResult>>

    @Transaction
    open fun insertAllResults(result: List<DebateGameResult>) {
        for (debateGameResult in result) {
            insert(debateGameResult)
        }
    }

}