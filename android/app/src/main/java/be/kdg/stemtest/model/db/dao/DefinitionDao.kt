package be.kdg.stemtest.model.db.dao

import androidx.room.Dao
import androidx.room.Insert
import androidx.room.Query
import androidx.room.Transaction
import be.kdg.stemtest.model.entity.Definition
import io.reactivex.Single


@Dao
abstract class DefinitionDao {

    @Insert
    abstract fun insert(debateGameResult: Definition)

    @Query("SELECT * FROM definition")
    abstract fun getDefinitions(): Single<List<Definition>>


    @Transaction
    open fun insertAllDefinitions(definitions: List<Definition>) {
        for (definition in definitions) {
            insert(definition)
        }
    }
}